package com.bugzappr.mbtimatchr.dto;

import java.util.Objects;
import java.util.UUID;

public class QueueResponse {
  private UUID uuid;
  private String matched_mbti;
  private UUID matched_uuid;
  private String server_host;
  private int server_port;
  private int gameroom_index;

  public QueueResponse(UUID uuid, String matched_mbti, UUID matched_uuid, String server_host,
      int server_port, int gameroom_index) {
    this.uuid = uuid;
    this.matched_mbti = matched_mbti;
    this.matched_uuid = matched_uuid;
    this.server_host = server_host;
    this.server_port = server_port;
    this.gameroom_index = gameroom_index;
  }

  public UUID getUuid() {
    return uuid;
  }

  public void setUuid(UUID uuid) {
    this.uuid = uuid;
  }

  public String getMatched_mbti() {
    return matched_mbti;
  }

  public void setMatched_mbti(String matched_mbti) {
    this.matched_mbti = matched_mbti;
  }

  public UUID getMatched_uuid() {
    return matched_uuid;
  }

  public void setMatched_uuid(UUID matched_uuid) {
    this.matched_uuid = matched_uuid;
  }

  public String getServer_host() {
    return server_host;
  }

  public void setServer_host(String server_host) {
    this.server_host = server_host;
  }

  public int getServer_port() {
    return server_port;
  }

  public void setServer_port(int server_port) {
    this.server_port = server_port;
  }

  public int getGameroom_index() {
    return gameroom_index;
  }

  public void setGameroom_index(int gameroom_index) {
    this.gameroom_index = gameroom_index;
  }

  @Override
  public int hashCode() {
    return Objects.hash(gameroom_index, matched_mbti, matched_uuid, server_host, server_port, uuid);
  }

  @Override
  public boolean equals(Object obj) {
    if (this == obj)
      return true;
    if (obj == null)
      return false;
    if (getClass() != obj.getClass())
      return false;
    QueueResponse other = (QueueResponse) obj;
    return gameroom_index == other.gameroom_index
        && Objects.equals(matched_mbti, other.matched_mbti)
        && Objects.equals(matched_uuid, other.matched_uuid)
        && Objects.equals(server_host, other.server_host) && server_port == other.server_port
        && Objects.equals(uuid, other.uuid);
  }
}
