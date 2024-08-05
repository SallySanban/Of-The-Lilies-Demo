using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableStore
{
    private const string defaultDatabaseName = "Default";
    public const char databaseVariableId = '.';
    public static readonly string regexVariableIds = @"[!]?\$[a-zA-Z0-9_.]+";
    public const char variableId = '$';

    private static Dictionary<string, Database> databases = new Dictionary<string, Database>()
    {
        { defaultDatabaseName, new Database(defaultDatabaseName) }
    };
    private static Database defaultDatabase => databases[defaultDatabaseName];

    public class Database
    {
        public string name;
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

        public Database(string name)
        {
            this.name = name;
            variables = new Dictionary<string, Variable>();
        }
    }

    public abstract class Variable
    {
        public abstract object Get();
        public abstract void Set(object value);
    }

    public class Variable<T> : Variable
    {
        private T value;
        private Func<T> getter;
        private Action<T> setter;

        public Variable(T defaultValue = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultValue;

            if(getter == null)
            {
                this.getter = () => value;
            }
            else
            {
                this.getter = getter;
            }

            if(setter == null)
            {
                this.setter = newValue => value = newValue;
            }
            else
            {
                this.setter = setter;
            }
        }

        public override object Get() => getter();

        public override void Set(object newValue) => setter((T)newValue);
    }

    public static bool CreateDatabase(string name)
    {
        if (!databases.ContainsKey(name))
        {
            databases[name] = new Database(name);
            return true;
        }

        return false;
    }

    public static Database GetDatabase(string name)
    {
        if(name == string.Empty)
        {
            return defaultDatabase;
        }

        if (!databases.ContainsKey(name))
        {
            CreateDatabase(name);
        }

        return databases[name];
    }

    public static void PrintDatabases()
    {
        foreach(KeyValuePair<string, Database> db in databases)
        {
            Debug.Log($"Database: {db.Key}");
        }
    }

    public static void PrintVariables(Database database = null)
    {
        if(database != null)
        {
            PrintDatabaseVariables(database);
            return;
        }

        foreach (var db in databases)
        {
            PrintDatabaseVariables(db.Value);
        }
    }

    private static void PrintDatabaseVariables(Database database)
    {
        foreach (KeyValuePair<string, Variable> variablePair in database.variables)
        {
            string variableName = variablePair.Key;
            object variableValue = variablePair.Value.Get();
            Debug.Log($"Database: {database.name}, Variable Name: {variableName}, Value: {variableValue}");
        }
    }

    public static bool CreateVariable<T>(string name, T defaultValue, Func<T> getter = null, Action<T> setter = null)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (db.variables.ContainsKey(variableName))
        {
            return false;
        }

        db.variables[variableName] = new Variable<T>(defaultValue, getter, setter);

        return true;
    }

    public static bool HasVariable(string name)
    {
        string[] parts = name.Split(databaseVariableId);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return db.variables.ContainsKey(variableName);
    }

    public static void RemoveAllVariables()
    {
        databases.Clear();
        databases[defaultDatabaseName] = new Database(defaultDatabaseName);
    }

    public static void RemoveVariable(string name)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if(db.variables.ContainsKey(variableName))
        {
            db.variables.Remove(variableName);
        }
    }

    public static bool TryGetValue(string name, out object variable)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (!db.variables.ContainsKey(variableName))
        {
            variable = null;
            return false;
        }

        variable = db.variables[variableName].Get();

        return true;
    }

    public static bool TrySetValue<T>(string name, T value)
    {
        (string[] parts, Database db, string variableName) = ExtractInfo(name);

        if (!db.variables.ContainsKey(variableName))
        {
            return false;
        }

        db.variables[variableName].Set(value);
        return true;
    }

    private static (string[], Database, string) ExtractInfo(string name)
    {
        string[] parts = name.Split(databaseVariableId);
        Database db = parts.Length > 1 ? GetDatabase(parts[0]) : defaultDatabase;
        string variableName = parts.Length > 1 ? parts[1] : parts[0];

        return (parts, db, variableName);
    }
}
