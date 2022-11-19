package com.bugzappr.mbtimatchr.controller;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class MBTIMapping {
  public static final Map mapping = new HashMap<String, List<String>>();

  static{
    mapping.put("INFP", new ArrayList<String>(Arrays.asList("INFP", "ENFP")));
    mapping.put("ENFP", new ArrayList<String>(Arrays.asList("INFP", "ENFP")));
    mapping.put("ESFJ", new ArrayList<String>(Arrays.asList("ISFJ", "ESFJ")));
    mapping.put("ISFJ", new ArrayList<String>(Arrays.asList("ISFJ", "ESFJ")));
  }
}
