using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

using CSVTable = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, string>>;
using CacheTable = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.Dictionary<int, object>>;

public class DataTable {

    static CacheTable cache = new CacheTable();

    public static T Get<T>(int id) {
        var type = typeof(T);
        if (!cache.ContainsKey(type)) {
            cache[type] = Load<T>("DataTable/" + type.Name);
        }
        T data = (T)cache[type][id];
        return data;
    }

    private static Dictionary<int, object> Load<T>(string path) {
        Dictionary<int, object> datas = new Dictionary<int, object>();
        var textAsset = Resources.Load<TextAsset>(path);
        var table = Parser(textAsset.text);

        FieldInfo[] fields = typeof(T).GetFields();
        foreach (var id in table.Keys) {
            var row = table[id];

            var obj = Activator.CreateInstance<T>();

            foreach (FieldInfo fi in fields) {
                fi.SetValue(obj, Convert.ChangeType(row[fi.Name], fi.FieldType));
            }
            datas[Convert.ToInt32(id)] = obj;
        }

        return datas;
    }

    private static CSVTable Parser(string content) {

        CSVTable result = new CSVTable();
        
        string[] row = content.Replace("\r", "").Split(new char[] { '\n' });
        string[] columnHeads = row[0].Split(new char[] { ',' });


        for (int i = 1; i < row.Length; i++) {
            string[] line = row[i].Split(new char[] { ',' });
            var id = line[0];
            if (String.IsNullOrEmpty(id)) break;

            result[id] = new Dictionary<string, string>();
            for(int j = 0; j < line.Length; j++) {
                result[id][columnHeads[j]] = line[j];
            }
        }

        return result;
    }
}
