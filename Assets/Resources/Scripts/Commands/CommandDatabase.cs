using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDatabase
{
    private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

    public bool HasCommand(string commandName) => database.ContainsKey(commandName);

    public void AddCommand(string commandName, Delegate command)
    {
        if (!database.ContainsKey(commandName))
        {
            database.Add(commandName, command);
        }
        else
        {
            Debug.LogError($"Command {commandName} already exists in the database");
        }
    }

    public Delegate GetCommand(string commandName)
    {
        if (!database.ContainsKey(commandName))
        {
            Debug.LogError($"Command {commandName} does not exist in the database");
            return null;
        }

        return database[commandName];
    }
}
