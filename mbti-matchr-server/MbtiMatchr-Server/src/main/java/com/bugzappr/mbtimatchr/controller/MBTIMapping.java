package com.bugzappr.mbtimatchr.controller;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class MBTIMapping {
  public static final Map<String, List<String>> mapping = new HashMap<String, List<String>>();

  static{
    mapping.put("INFP", new ArrayList<String>(Arrays.asList("INFP", "ENFP", "INFJ", "ENFJ", "INTJ", "ENTJ", "INTP", "ENTP")));
    mapping.put("ENFP", new ArrayList<String>(Arrays.asList("INFP", "ENFP")));
    mapping.put("ISFP", new ArrayList<String>(Arrays.asList("ISFP", "ENFJ", "ESFJ", "ESTJ")));
    mapping.put("ESFP", new ArrayList<String>(Arrays.asList("ESFP", "ISFJ", "ISTJ")));
    mapping.put("ISTP", new ArrayList<String>(Arrays.asList("ISTP", "ESFJ", "ESTJ")));
    mapping.put("ESTP", new ArrayList<String>(Arrays.asList("ESTP", "ISFJ", "ISTJ")));
    mapping.put("ISFJ", new ArrayList<String>(Arrays.asList("ISFJ", "ESFJ")));
    mapping.put("ESFJ", new ArrayList<String>(Arrays.asList("ISFJ", "ESFJ")));
  }
}
