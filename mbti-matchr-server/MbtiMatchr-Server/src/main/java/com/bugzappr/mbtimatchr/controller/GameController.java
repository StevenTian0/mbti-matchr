package com.bugzappr.mbtimatchr.controller;

import java.util.ArrayList;
import java.util.List;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/game")
public class GameController {
  private List<String[][]> rooms = new ArrayList<String[][]>();
  //private String[] ids = new String[2];
  //private String[] jsons = new String [2];

  @PostMapping("/post")
  public String postJson(@RequestParam String pid, @RequestParam int roomIndex, @RequestParam String data){
    if(pid.equals(rooms.get(roomIndex)[0])){
      jsons[0] = data;
      return jsons[1];
    }else{
      jsons[1] = data;
      return jsons[0];
    }
  }

}
