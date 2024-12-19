using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Commands
{
    public class DatabaseExtensionGeneral : CommandDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("Wait", new Func<string, IEnumerator>(Wait));
        }

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }
    }
}

