package com.bugzappr.mbtimatchr.controller;

public class Player {
  private String name;

  private Player match;
  public Player(String name){
    this.name = name;
  }

  public String getName() {
    return name;
  }

  public Player getMatch() {
    return match;
  }

  public void setMatch(Player match) {
    this.match = match;
  }
}
