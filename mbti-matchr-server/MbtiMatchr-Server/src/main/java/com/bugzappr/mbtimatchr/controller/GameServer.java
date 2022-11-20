package com.bugzappr.mbtimatchr.controller;
import java.io.ByteArrayInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.*;
import java.nio.charset.StandardCharsets;

public class GameServer {
  private ServerSocket ss;
  private int numPlayers;
  private Socket p1Socket;
  private Socket p2Socket;
  private ReadFromClient p1Read;
  private ReadFromClient p2Read;
  private WriteToClient p1Write;
  private WriteToClient p2Write;

  private String p1Json, p2Json;

  public GameServer(){
    try{
      ss = new ServerSocket(51234);
    } catch (IOException e) {
      System.out.println(e);
    }
  }

  private class ReadFromClient implements Runnable{
    private int pid;
    private ByteArrayInputStream dataIn;

    public ReadFromClient(int pid, ByteArrayInputStream in){
      this.pid = pid;
      dataIn = in;
    }

    @Override
    public void run() {
      try{
        while(true){
          if(pid == 1){
            p1Json = new String(dataIn.readAllBytes(), StandardCharsets.UTF_8);
          }else{
            p2Json = new String(dataIn.readAllBytes(), StandardCharsets.UTF_8);
          }
        }
      }catch(Exception e){

      }
    }
  }

  private class WriteToClient implements Runnable{
    private int pid;
    private DataOutputStream dataOut;

    public WriteToClient(int pid, DataOutputStream out){
      this.pid = pid;
      dataOut = out;
    }

    @Override
    public void run() {
      try{
        while(true){
          if(pid == 1){
            dataOut.writeBytes(p2Json);
            dataOut.flush();
          }else{
            dataOut.writeBytes(p1Json);
            dataOut.flush();
          }
        }
      }catch(Exception e){

      }
    }
  }

  public void acceptConnections() throws IOException {
    try {
      while (numPlayers <= 2) {
        Socket s = ss.accept();
        ByteArrayInputStream in = new ByteArrayInputStream(s.getInputStream().readAllBytes());
        DataOutputStream out = new DataOutputStream(s.getOutputStream());

        numPlayers++;
        out.writeInt(numPlayers);

        ReadFromClient rfc = new ReadFromClient(numPlayers, in);
        WriteToClient wtc = new WriteToClient(numPlayers, out);

        if(numPlayers == 1){
          p1Socket = s;
          p1Read = rfc;
          p1Write = wtc;
        }else{
          p2Socket = s;
          p2Read = rfc;
          p2Write = wtc;
        }
      }
    } catch (IOException e) {
      System.out.println(e);
    }
  }

  public static void main(String[] args) throws IOException {
    GameServer gc = new GameServer();
    gc.acceptConnections();
  }
}
