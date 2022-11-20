package com.bugzappr.mbtimatchr.controller;


import java.util.UUID;
import java.util.concurrent.ConcurrentLinkedDeque;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.locks.ReentrantLock;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.context.request.async.DeferredResult;
import com.bugzappr.mbtimatchr.dto.QueueResponse;
import com.bugzappr.mbtimatchr.model.QueuePlayer;

@RestController
@RequestMapping("/api")
public class SessionController {
  private final ConcurrentLinkedDeque<QueuePlayer> queue = new ConcurrentLinkedDeque<QueuePlayer>();
  private final ExecutorService matchers = Executors.newFixedThreadPool(6);

  private static final String SERVER_HOST = "192.168.137.1";
  private static final Integer SERVER_PORT = 51234;

  private Integer current_gameroom_index = 0;

  private final ReentrantLock lock = new ReentrantLock();

  @PostMapping("/join")
  public DeferredResult<QueueResponse> join(@RequestParam String mbti) {
    DeferredResult<QueueResponse> output = new DeferredResult<>(10000L);
    QueuePlayer player = new QueuePlayer(mbti, UUID.randomUUID());
    output.onTimeout(() -> {
      synchronized (player) {
        player.notify();
        output.setResult(new QueueResponse(player.getUuid(), null, null, null, 0, 0));
      }
    });
    matchers.execute(() -> {
      synchronized (player) {
        if (queue.isEmpty()) {
          try {
            queue.add(player);
            // accessing shared variable
            lock.lock();
            try {
              player.setGameroomIndex(current_gameroom_index);
              current_gameroom_index++;
            } finally {
              lock.unlock();
            }
            player.wait();
            queue.remove(player);
            if (player.getMatch() != null) {
              output.setResult(new QueueResponse(player.getUuid(), player.getMatch().getMbti(),
                  player.getMatch().getUuid(), SERVER_HOST, SERVER_PORT,
                  player.getGameroomIndex()));
            }
          } catch (Exception e) {
            System.out.println("1 " + e);
            output.setErrorResult(e.getMessage());
          }
        } else {
          try {
            boolean found = false;
            for (QueuePlayer p : queue) {
              if (MBTIMapping.mapping.get(mbti).contains(p.getMbti())) {
                found = true;
                p.setMatch(player);
                player.setMatch(p);
                player.setGameroomIndex(p.getGameroomIndex());
                synchronized (p) {
                  p.notify();
                }
                queue.remove(player);
                output.setResult(new QueueResponse(player.getUuid(), player.getMatch().getMbti(),
                    player.getMatch().getUuid(), SERVER_HOST, SERVER_PORT,
                    player.getGameroomIndex()));
              }
            }
            if (!found) {
              queue.add(player);
              lock.lock();
              try {
                player.setGameroomIndex(current_gameroom_index);
                current_gameroom_index++;
              } finally {
                lock.unlock();
              }
              player.wait();
              queue.remove(player);
              if (player.getMatch() != null) {
                output.setResult(new QueueResponse(player.getUuid(), player.getMatch().getMbti(),
                    player.getMatch().getUuid(), SERVER_HOST, SERVER_PORT,
                    player.getGameroomIndex()));
              }
            }
          } catch (Exception e) {
            System.out.println("2" + e);
            output.setErrorResult(e.getMessage());
          }
        }
      }
    });
    return output;
  }
}
