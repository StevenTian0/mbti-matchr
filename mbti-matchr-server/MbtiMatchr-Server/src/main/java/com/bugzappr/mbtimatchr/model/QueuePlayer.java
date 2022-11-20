package com.bugzappr.mbtimatchr.model;

import java.util.Objects;
import java.util.UUID;

public class QueuePlayer {
  private String mbti;
  private UUID uuid;
  private QueuePlayer match;
  private int gameroomIndex;

  public QueuePlayer(String mbti, UUID uuid) {
    this.mbti = mbti;
    this.uuid = uuid;
    this.match = null;
    this.gameroomIndex = -1;
  }

  public int getGameroomIndex() {
    return gameroomIndex;
  }

  public void setGameroomIndex(int gameroomIndex) {
    this.gameroomIndex = gameroomIndex;
  }

  public String getMbti() {
    return mbti;
  }

  public UUID getUuid() {
    return uuid;
  }

  public QueuePlayer getMatch() {
    return match;
  }

  public void setMatch(QueuePlayer match) {
    this.match = match;
  }

  @Override
  public int hashCode() {
    return Objects.hash(uuid);
  }

  @Override
  public boolean equals(Object obj) {
    if (this == obj)
      return true;
    if (obj == null)
      return false;
    if (getClass() != obj.getClass())
      return false;
    QueuePlayer other = (QueuePlayer) obj;
    return Objects.equals(uuid, other.uuid);
  }
}
