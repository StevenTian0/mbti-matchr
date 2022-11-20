package com.bugzappr.mbtimatchr.controller;

import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedDeque;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.locks.ReentrantLock;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.context.request.async.DeferredResult;
import com.bugzappr.mbtimatchr.model.GamePlayer;

@RestController
@RequestMapping("/game")
public class GameController {

  private ConcurrentHashMap<Integer, ConcurrentLinkedDeque<GamePlayer>> rooms =
      new ConcurrentHashMap<Integer, ConcurrentLinkedDeque<GamePlayer>>();

  private final ExecutorService waitingForOther = Executors.newFixedThreadPool(6);

  private final ReentrantLock lock = new ReentrantLock();

  @PostMapping("/ready")
  public DeferredResult<Integer> ready(@RequestParam Integer roomId) {
    DeferredResult<Integer> output = new DeferredResult<>(10000L);
    GamePlayer player = new GamePlayer(roomId, "{}");
    output.onTimeout(() -> {
      synchronized (player) {
        player.notify();
        output.setResult(-1); // abandon matching
      }
    });
    waitingForOther.execute(() -> {
      synchronized (player) {
        ConcurrentLinkedDeque<GamePlayer> room = rooms.get(roomId);
        if (room == null) {
          try {
            var newRoom = new ConcurrentLinkedDeque<GamePlayer>();
            newRoom.add(player);
            rooms.put(roomId, newRoom);
            player.wait();
            // player 1 has been notified
            if (newRoom.size() == 2) {
              output.setResult(1);
            }
          } catch (Exception e) {
            System.out.println("GameController 1" + e);
            output.setErrorResult(e.getMessage());
          }
        } else {
          try {
            room.add(player);
            var firstPlayer = room.getFirst();
            synchronized (firstPlayer) {
              firstPlayer.notify();
            }
            output.setResult(2);
          } catch (Exception e) {
            System.out.println("GameController 2" + e);
          }
        }
      }
    });
    return output;
  }

  @PostMapping("/update")
  public String playerUpdate(@RequestParam Integer roomId, @RequestParam Integer pid,
      @RequestParam String data) {
    var room = rooms.get(roomId);
    if (pid == 1) {
      lock.lock();
      try {
        room.getFirst().setData(data);
      } finally {
        lock.unlock();
      }
      return room.getLast().getData();
    } else {
      lock.lock();
      try {
        room.getLast().setData(data);
      } finally {
        lock.unlock();
      }
      return room.getFirst().getData();
    }
  }

}
