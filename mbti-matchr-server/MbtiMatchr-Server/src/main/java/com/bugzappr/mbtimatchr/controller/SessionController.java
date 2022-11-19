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
            for(Player p : queue) {
              if (((ArrayList<String>) MBTIMapping.mapping.get("name")).contains(p.getName())) ;
              queue.remove(p);
              output.setResult(
                  format("Paired with %s. Enjoy!", p.getName()));
            }
            player.wait();
          } catch (Exception e) {
            System.out.println(e.getMessage());
            output.setErrorResult(e.getMessage());
          }
        } else {
          try {
            for(Player p : queue) {
              if (((ArrayList<String>) MBTIMapping.mapping.get("name")).contains(p.getName())) ;
              queue.remove(p);
              output.setResult(
                  format("Paired with %s. Enjoy!", p.getName()));
            }
            output.setResult(
                format("Paired with %s. Enjoy!", queue.poll()));
            queue.add(name);
            monitor.notify();
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
