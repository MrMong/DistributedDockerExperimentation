using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DistributedExperimentation.Experimenter.Application;

namespace DistributedExperimentation.Experimenter.UI
{
    class ApplicationMain
    {
        private static String defaultPluginFilePrefix = "ExecutorPlugin";
        private static String defaultPluginDirPath = Directory.GetCurrentDirectory();

        public static int Main(string[] args)
        {
            int exitCode = 0;
            try {
                Dictionary<string, string> parsedArgs = parseArguments(args);
                if (parsedArgs.ContainsKey("json-schema")) {
                    Console.WriteLine(Application.Experimenter.getCurrentJsonSchema());
                } else {
                    String jsonContent = parseJsonContent(parsedArgs.Where(x => 
                        {
                            return ((String.Compare(x.Key,"-e") == 0) || 
                                    (String.Compare(x.Key,"--experiment-data") == 0));
                        }).First().Value);
                    String pluginPath = parsePluginFilePath(parsedArgs.Where(x => 
                        {
                            return ((String.Compare(x.Key,"-p") == 0) || 
                                    (String.Compare(x.Key,"--plugin-path") == 0));
                        }).FirstOrDefault().Value);
                    ExecutorPluginProxy proxy = ExecutorPluginProxy.create(pluginPath);
                    Application.Experimenter experimenter = Application.Experimenter.create();
                    experimenter.executeExperimentation(jsonContent, proxy);
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                exitCode = -1;
            }
            return exitCode;
        }

        private static Dictionary<string, string> parseArguments(string[] args)
        {
            try {
                if (!((args.Length == 1) || 
                      (args.Length == 2) ||
                      (args.Length == 4)))
                    throw new Exception();
                for(int i=0; i<args.Length; i++) {
                    args[i] = args[i].Trim();
                    if (args[i].First().Equals('\''))
                        args[i] = args[i].Trim('\'');
                    else if (args[i].First().Equals('\"'))
                        args[i] = args[i].Trim('"');
                }
                Dictionary<string, string> parsedArgs = null;
                if (String.Compare(args[0], "json-schema") == 0) {
                    parsedArgs = new Dictionary<string, string>();
                    parsedArgs.Add("json-schema","");
                } else {
                    parsedArgs = args.Select((s, i) => new {s, i})
                                    .GroupBy(x => x.i / 2)
                                    .ToDictionary(g => g.First().s, g => g.Last().s);
                    if ((parsedArgs.Count == 1) || (parsedArgs.Count == 2)) {
                        if ((!parsedArgs.ContainsKey("-e")) &&
                            (!parsedArgs.ContainsKey("--experiment-data")))
                            throw new Exception();
                        if (parsedArgs.Count == 2) {
                            if ((!parsedArgs.ContainsKey("-p")) &&
                                (!parsedArgs.ContainsKey("--plugin-path")))
                                throw new Exception();
                        }
                    } else {
                        throw new Exception();
                    }
                }              
                return parsedArgs;           
            } catch (Exception ex) {
                throw new System.ArgumentException("Given arguments\n   " +
                                                   String.Join(" ", args) +"\n" +
                                                   "are not correct.\n \n" +
                                                   createHelpText(defaultPluginFilePrefix, 
                                                                  defaultPluginDirPath), 
                                                   ex);
            }
        }

        private static String parseJsonContent(String data)
        {
            String jsonContent = null;
            if (data[0] == '@') {
                jsonContent = readFile(data.TrimStart('@'));
            } else {
                jsonContent = data;
            }
            return jsonContent;
        }   

        private static String parsePluginFilePath(String filepath) 
        {
            if (!String.IsNullOrEmpty(filepath)) {
                filepath = filepath.TrimStart('@');
                readFile(filepath);
                String filename = Path.GetFileNameWithoutExtension(filepath);
                bool isOk = false;
                if (filename.Length >= defaultPluginFilePrefix.Length) {
                    String prefix = filename.Substring(0,defaultPluginFilePrefix.Length);
                    isOk = (String.Compare(prefix, defaultPluginFilePrefix) == 0);
                }
                if (!isOk) {
                    throw new ArgumentException("The file name of the given plugin path\n   " +
                                                filepath + "\ndo not offers the required prefix\n  " + 
                                                defaultPluginFilePrefix);
                } 
            } else {
                filepath = searchPluginFile(defaultPluginFilePrefix, 
                                            defaultPluginDirPath);
            }
            return filepath;
        }   

        private static String readFile(String filepath) 
        {
            String result = null;
            String error = "The given file path\n  " + filepath + "\nis invalid, because:\n";
            try{
                result = File.ReadAllText(filepath);
             } catch (ArgumentException aex) { 
                throw new ArgumentException(error + "The given file path is null or empty.", aex);                 
             } catch (PathTooLongException pex) {
                 throw new ArgumentException(error + "The given file path exceed the " + 
                                             "system-defined maximum length.", pex); 
             } catch (DirectoryNotFoundException dex) {
                 throw new ArgumentException(error + "The specified file path cannot be found.", dex);
             } catch(FileNotFoundException fex) {
                 throw new ArgumentException(error + "The specified file cannot be found.", fex);              
             } catch (IOException ioex) {
                 throw new ArgumentException(error + "An I/O error occurred " +
                                             "while opening the file.", ioex);
             } catch(UnauthorizedAccessException uex) {
                 throw new ArgumentException(error + "The given file cannot be accessed " +
                                             "because no permission.", uex);
             } catch(NotSupportedException nex) {
                 throw new ArgumentException(error + "The given file path is " +
                                             "in an invalid format.", nex);
             } catch(System.Security.SecurityException scex) {
                 throw new ArgumentException(error + "The program does not have the required " +
                                             "permission to read the specified file.", scex);
             } 
             return result;
        }         

        private static String searchPluginFile(String pluginFilePrefix, String pluginDirPath) {
            bool isOk = ((!String.IsNullOrEmpty(pluginFilePrefix)) &&
                        (!String.IsNullOrEmpty(pluginDirPath)));
            if (!isOk)
                throw new ArgumentException("One or more given arguments are not valid.\n" +
                                            "pluginFilePrefix must be a valid non empty string\n"+
                                            "pluginDirPath must be a valid non empty string");
            string[] pluginFiles = Directory.GetFiles(pluginDirPath, pluginFilePrefix + "*.dll");
            if (pluginFiles.Length < 1)
                throw new FileNotFoundException("Could not found a plugin file with the given filename " +
                                                "prefix\n  " + pluginFilePrefix + "\nin the given directory\n  " + 
                                                pluginDirPath);
            return pluginFiles[0];
        }         

        private static String createHelpText(String pluginFilePrefix, String pluginDirPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Usage:\t Experimenter.UI");
            sb.Append("\n  -e, --experiment-data\t\t");
            sb.Append("json data of an series of experiments or path to json file, which contains the data\n");
            sb.Append("\t\t\t\tfile path must be escaped with an @ sign\n");              
            sb.Append("\n [-p, --plugin-path]\t\t");
            sb.Append("path to plugin file, which is needed to trigger the experiment software\n");
            sb.Append("\t\t\t\tname of plugin file must be have the prefix: " + defaultPluginFilePrefix + "\n");
            sb.Append("\t\t\t\tby default, it will be searched for an plugin file as follows:\n");
            sb.Append("\t\t\t\t   search directory:\t");
            sb.Append(pluginDirPath + "\n");
            sb.Append("\t\t\t\t   search file pattern:\t");
            sb.Append(pluginFilePrefix + "*.dll\n\n");
            sb.Append(" \n");
            sb.Append("\nUsage:\t Experimenter.UI json-schema\n");
            sb.Append("\t Return the currently used json schema for the semantical validation.");            
            return sb.ToString();
        }  
    }
}
