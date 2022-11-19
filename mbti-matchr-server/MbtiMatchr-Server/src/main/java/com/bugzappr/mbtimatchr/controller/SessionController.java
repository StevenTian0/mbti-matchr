package com.bugzappr.mbtimatchr.controller;

import static java.lang.String.format;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.context.request.async.DeferredResult;

@RestController
@RequestMapping("/api")
public class SessionController {
  private ExecutorService bakers = Executors.newFixedThreadPool(5);

  @GetMapping("/bake/{bakedGood}")
  public DeferredResult<String> publisher(@PathVariable String bakedGood, @RequestParam Integer bakeTime) {
    DeferredResult<String> output = new DeferredResult<>(20000L);
    output.onTimeout(() -> output.setErrorResult("the bakery is not responding in allowed time"));
    bakers.execute(() -> {
      try {
        Thread.sleep(bakeTime);
        output.setResult(format("Bake for %s complete and order dispatched. Enjoy!", bakedGood));
      } catch (Exception e) {
        output.setErrorResult("Something went wrong with your order!");
      }
    });
    return output;
  }

  @GetMapping("/hello")
  public String hello() {
    return "output";
  }
}
