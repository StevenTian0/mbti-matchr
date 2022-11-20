package com.bugzappr.mbtimatchr.model;

public class GamePlayer {
  private int gameroom;
  private String data;

  public GamePlayer(int gameroom, String data) {
    this.gameroom = gameroom;
    this.data = data;
  }

  public int getGameroom() {
    return gameroom;
  }

  public String getData() {
    return data;
  }

  public void setData(String data) {
    this.data = data;
  }
}
