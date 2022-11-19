package com.bugzappr.mbtimatchr.controller;

import static java.lang.String.format;

import java.util.ArrayDeque;
import java.util.ArrayList;
import java.util.List;
import java.util.Queue;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.context.request.async.DeferredResult;

@RestController
@RequestMapping("/api")
public class SessionController {
  private List<Player> queue = new ArrayList<Player>();
  private ExecutorService bakers = Executors.newFixedThreadPool(5);

  @GetMapping("/join/{name}")
  public DeferredResult<String> join(@PathVariable String name) {
    DeferredResult<String> output = new DeferredResult<>(20000L);
    output.onTimeout(() -> {queue.remove(name);output.setErrorResult("please try later");});
    Player player = new Player(name);
    bakers.execute(() -> {
      synchronized (player) {
        if (queue.isEmpty()) {
          try {
            queue.add(player);
            player.wait();
            queue.remove(player);
            output.setResult(
                format("Paired with %s. Enjoy!", player.getMatch().getName()));
          } catch (Exception e) {
            System.out.println(e.getMessage());
            output.setErrorResult(e.getMessage());
          }
        } else {
          try {
            boolean found = false;
            for(Player p : queue) {
              if (((ArrayList<String>) MBTIMapping.mapping.get(name)).contains(p.getName())) {
                found = true;
                p.setMatch(player);
                synchronized (p) {
                  p.notify();
                }
                queue.remove(player);
                output.setResult(
                    format("Paired with %s. Enjoy!", p.getName()));
              }
            }
            if(!found) {
              queue.add(player);
              player.wait();
              queue.remove(player);
              output.setResult(
                  format("Paired with %s. Enjoy!", player.getMatch().getName()));
            }
          } catch (Exception e) {
            System.out.println(e.getMessage());
            output.setErrorResult(e.getMessage());
          }
        }
      }
    });
    return output;
  }
}
