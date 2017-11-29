using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class EditorBuiltinIcon {
    public static string gray = "sv_label_0";
    public static string blue = "sv_label_1";
    public static string teal = "sv_label_2";
    public static string green = "sv_label_3";
    public static string yellow = "sv_label_4";
    public static string orange = "sv_label_5";
    public static string red = "sv_label_6";
    public static string purple = "sv_label_7";

    public static string circleGray = "sv_icon_dot0_pix16_gizmo";
    public static string circleBlue = "sv_icon_dot1_pix16_gizmo";
    public static string circleTeal = "sv_icon_dot2_pix16_gizmo";
    public static string circleGreen = "sv_icon_dot3_pix16_gizmo";
    public static string circleYellow = "sv_icon_dot4_pix16_gizmo";
    public static string circleOrange = "sv_icon_dot5_pix16_gizmo";
    public static string circleRed = "sv_icon_dot6_pix16_gizmo";
    public static string circlePurple = "sv_icon_dot7_pix16_gizmo";

    public static string diamondGray = "sv_icon_dot8_pix16_gizmo";
    public static string diamondBlue = "sv_icon_dot9_pix16_gizmo";
    public static string diamondTeal = "sv_icon_dot10_pix16_gizmo";
    public static string diamondGreen = "sv_icon_dot11_pix16_gizmo";
    public static string diamondYellow = "sv_icon_dot12_pix16_gizmo";
    public static string diamondOrange = "sv_icon_dot13_pix16_gizmo";
    public static string diamondRed = "sv_icon_dot14_pix16_gizmo";
    public static string diamondPurple = "sv_icon_dot15_pix16_gizmo";
}

public static class EditorIconUtil {

    public static Dictionary<string, GUIContent> iconContents = new Dictionary<string, GUIContent>();

    public static void SetGameObjectIcon(GameObject go, string icon) {
        if (!iconContents.ContainsKey(icon)) {
            iconContents[icon] = EditorGUIUtility.IconContent(icon);
        }

        var type = typeof(EditorGUIUtility);
        var mi = type.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        mi.Invoke(null, new object[] { go, iconContents[icon].image });
    }
}