﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HierarchyAnalyzer
{
    internal class Matcher
    {
        private const string TAG = "Matcher";

        internal static bool IsJavaClassDeclarationLine(string line)
        {
            Regex regex = new Regex(@"(class)\s\w*(\s+?(extends))*");
            Match m = regex.Match(line);

            if (m.Success == true)
                return true;
            else return false;
        }

        internal static bool IsJavaMethodDeclarationLine(string line)
        {
            Regex regex = new Regex(@"(public|protected|private|static|\s)(\s+final)* +[\w\<\>\[\]]+\s+(\w+) *\([^\)]*\) *(\{?|[^;])");
            Match m = regex.Match(line);

            if (m.Success == true)
                return true;
            else return false;
        }

        internal static bool IsURIMetadataDeclarationLine(string line)
        => (line.Replace(" ", "").StartsWith("@") &&
            line.Replace(" ", "").EndsWith("\")") &&
            line.Count(x => x.ToString() == "\"") == 2) ? true : false;

        internal static bool IsJavaMethodUsageLine(string line, string targetContains)
        {
            if (line.Contains("Method not decompiled"))
                return false;
            else
                return (line.Contains("." + targetContains + "(")
                        &&
                      !(line.Replace(" ", "").StartsWith("private") ||
                        line.Replace(" ", "").StartsWith("public"))) ? true : false;
        }

        internal static Tuple<string, string, string>[] DoMatchesFromFile(string file, int curDepth = -1)
        {
            var matchedString = new List<Tuple<string, string, string>>();

            using (StreamReader sr = new StreamReader(file))
            {
                var tdata = sr.ReadLine();
                var line = 1;

                while (tdata != null)
                {
                    if (IsURIMetadataDeclarationLine(tdata))
                    {
                        var uriValue = Parser.ExtractURIFromMetadataDeclarationLine(tdata);

                        if (uriValue != null)
                        {
                            tdata = sr.ReadLine();

                            var rcValue = Parser.ExtractReferenceClassTypeDeclarationLine(tdata);
                            var cmValue = Parser.ExtractReferenceCallingMethodDeclarationLine(tdata);

                            if (rcValue != null)
                            {
                                matchedString.Add(new Tuple<string, string, string>(
                                    uriValue, rcValue, cmValue));
                            }
                        }
                    }

                    tdata = sr.ReadLine();
                    line++;
                }
            }

            if (matchedString.Count > 0)
                Console.WriteLine("{2}[i] {1} lines of matched found at: {0}",
                                   file, matchedString.Count,
                                   curDepth > 0 ? Logger.AddDepthToPrint(curDepth + 1) : "");
            else Console.WriteLine("{1}[i] no lines matched at: {0}",
                                   file,
                                   curDepth > 0 ? Logger.AddDepthToPrint(curDepth + 1) : "");

            return matchedString.ToArray();
        }

        internal static Tuple<string, string, string>[] DoClassMatchesFromFile(string file, int curDepth = -1)
        {
            var matchedString = new List<Tuple<string, string, string>>();

            using (StreamReader sr = new StreamReader(file))
            {
                var tdata = sr.ReadLine();
                var line = 1;

                while (tdata != null)
                {
                    if (IsURIMetadataDeclarationLine(tdata))
                    {
                        var uriValue = Parser.ExtractURIFromMetadataDeclarationLine(tdata);

                        if (uriValue != null)
                        {
                            tdata = sr.ReadLine();

                            var rcValue = Parser.ExtractReferenceClassTypeDeclarationLine(tdata);
                            var cmValue = Parser.ExtractReferenceCallingMethodDeclarationLine(tdata);

                            if (rcValue != null)
                            {
                                matchedString.Add(new Tuple<string, string, string>(
                                    uriValue, rcValue, cmValue));
                            }
                        }
                    }

                    tdata = sr.ReadLine();
                    line++;
                }
            }

            if (matchedString.Count > 0)
                Console.WriteLine("{2}[i] {1} lines of matched found at: {0}",
                                   file, matchedString.Count,
                                   curDepth > 0 ? Logger.AddDepthToPrint(curDepth + 1) : "");
            else Console.WriteLine("{1}[i] no lines matched at: {0}",
                                   file,
                                   curDepth > 0 ? Logger.AddDepthToPrint(curDepth + 1) : "");

            return matchedString.ToArray();
        }

        internal static string FindInterfaceNameOnFile(string file, int curDepth=-1)
        {
            string retStr = null;

            using (StreamReader sr = new StreamReader(file))
            {
                var tdata = sr.ReadLine();
                var line = 1;

                string interfaceString = "";

                while (tdata != null)
                {
                    if (tdata.Replace(" ", "").StartsWith("this.") &&
                        tdata.Replace(" ", "").EndsWith(".class);"))
                    {
                        string ddata = tdata.Split('(').Last();
                        interfaceString = ddata.Split(')').First().Split(".class").First();

                        //Console.WriteLine("{2}[i] found interface(name: {0}) at line {1}",
                        //                  interfaceString, line,
                        //                  curDepth > 0 ? Logger.AddDepthToPrint(curDepth) : "");

                        retStr = "interface " + interfaceString;
                        break;
                    }

                    tdata = sr.ReadLine();
                    line++;
                }
            }

            return retStr;
        }

    }
}
