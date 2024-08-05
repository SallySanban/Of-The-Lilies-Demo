using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVariableStore : MonoBehaviour
{
    public int var_int = 0;

    void Start()
    {
        VariableStore.CreateDatabase("numbers");
        VariableStore.CreateDatabase("boolean");
        VariableStore.CreateDatabase("links");

        VariableStore.CreateVariable("link_int", var_int, () => var_int, value => var_int = value);
        VariableStore.CreateVariable("numbers.num1", 1);
        VariableStore.CreateVariable("numbers.num2", 2);
        VariableStore.CreateVariable("boolean.lightIsOn", true);
        VariableStore.CreateVariable("numbers.float1", 0.5f);
        VariableStore.CreateVariable("str1", "hello");
        VariableStore.CreateVariable("str2", "world");

        VariableStore.PrintDatabases();

        VariableStore.PrintVariables();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            VariableStore.PrintVariables();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            string variable = "numbers.num1";
            VariableStore.TryGetValue(variable, out object v);
            VariableStore.TrySetValue(variable, (int)v + 5);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            VariableStore.TryGetValue("numbers.num1", out object num1);
            VariableStore.TryGetValue("numbers.num2", out object num2);

            Debug.Log($"num1 + num2 = {(int) num1 + (int)num2}");
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            VariableStore.TryGetValue("link_int", out object linkedInteger);
            VariableStore.TrySetValue("link_int", (int)linkedInteger + 5);
        }
    }
}
