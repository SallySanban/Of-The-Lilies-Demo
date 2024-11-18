//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Commands
//{
//    public class DatabaseExtensionProp : CommandDatabaseExtension
//    {
//        new public static void Extend(CommandDatabase database)
//        {
//            database.AddCommand("ShowProp", new Func<string, IEnumerator>(Show));
//            database.AddCommand("HideProp", new Func<IEnumerator>(Hide));
//        }

//        private static IEnumerator Show(string data)
//        {
//            Prop prop = PropManager.Instance.GetProp(data);

//            prop.Show();

//            while (prop.isPropShowing)
//            {
//                yield return null;
//            }
//        }

//        private static IEnumerator Hide()
//        {
//            Prop currentProp = PropManager.Instance.activeProp;

//            currentProp.Hide();

//            while (currentProp.isPropHiding)
//            {
//                yield return null;
//            }
//        }
//    }
//}

