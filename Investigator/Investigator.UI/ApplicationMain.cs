using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DistributedExperimentation.Investigator.UI
{
    // this class represent the presentation layer of investigator
    class ApplicationMain
    {
        static void Main(string[] args)
        {
            try {
                // example input to show usage of the ui interface
                // "-e", "@/path/to/exp1.json", 
                // "-d", "192.168.178.211:5000/experimenter-simple-adder-db-app-alpine3.7-noentry:1.0",
                // "-h", "unix:///var/run/docker.sock",
                // "-p", "/workdir/Experimenter/Experimenter.UI",
                // "-v", "1.39"
                Dictionary<string, string> parsedArgs = parseArguments(args);
                if (parsedArgs.ContainsKey("json-schema")) {
                    Console.WriteLine(Application.Investigator.getCurrentJsonSchema());
                } else {
                    String fileContent = parseJsonContent(parsedArgs.Where(x => 
                        {
                            return ((String.Compare(x.Key,"-e") == 0) || 
                                    (String.Compare(x.Key,"--experiment-data") == 0));
                        }).First().Value);
                    String imageName = parseDockerImageName(parsedArgs.Where(x => 
                        {
                            return ((String.Compare(x.Key,"-d") == 0) || 
                                    (String.Compare(x.Key,"--docker-image") == 0));
                        }).First().Value);
                    Uri dockerHost = parseDockerHostUri(parsedArgs.Where(x => 
                        {
                            return ((String.Compare(x.Key,"-h") == 0) || 
                                    (String.Compare(x.Key,"--docker-host") == 0));
                        }).First().Value);
                    String executionPath = parseExecutionPath(parsedArgs.Where(x => 
                        {
                            return ((String.Compare(x.Key,"-p") == 0) || 
                                    (String.Compare(x.Key,"--execution-path") == 0));
                        }).First().Value);
                    Version apiVersion = parseDockerRempoteApi(parsedArgs.Where(x => 
                        {
                            return ((String.Compare(x.Key,"-v") == 0) || 
                                    (String.Compare(x.Key,"--docker-api-version") == 0));
                        }).FirstOrDefault().Value);              
                    Application.Investigator investigator = Application.Investigator.create(dockerHost,
                                                                                        apiVersion);                    
                    Task task = investigator.startExperimentation(fileContent, 
                                                                  imageName, 
                                                                  executionPath);
                    task.Wait();
                }
            } catch (FormatException) {
                Console.WriteLine("The given content of experiment data contains " +
                                  "syntactically invalid JSON-Code.");
            } catch (ApplicationException) {
                Console.WriteLine("The given content of experiment data " +
                                  "violates the intern JSON schema.");                     
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        // parse and checks given arguments of ui interface
        private static Dictionary<string, string> parseArguments(string[] args)
        {
            try {
                if (!((args.Length == 1) || 
                      (args.Length == 8) ||
                      (args.Length == 10)))
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
                    if ((parsedArgs.Count == 4) || (parsedArgs.Count == 5)) {
                        if ((!parsedArgs.ContainsKey("-e")) &&
                            (!parsedArgs.ContainsKey("--experiment-data")))
                            throw new Exception();
                        if ((!parsedArgs.ContainsKey("-d")) &&
                            (!parsedArgs.ContainsKey("--docker-image")))
                            throw new Exception();
                        if ((!parsedArgs.ContainsKey("-h")) &&
                            (!parsedArgs.ContainsKey("--docker-host")))  
                            throw new Exception();   
                        if ((!parsedArgs.ContainsKey("-p")) &&
                            (!parsedArgs.ContainsKey("--execution-path")))                                      
                            throw new Exception();
                        if (parsedArgs.Count == 5) {
                            if ((!parsedArgs.ContainsKey("-v")) &&
                                (!parsedArgs.ContainsKey("--docker-api-version")))
                                throw new Exception();    
                        }
                    } else {
                        throw new Exception();
                    }
                }              
                return parsedArgs;           
            } catch (Exception ex) {
                throw new System.ArgumentException("Given arguments\n   " +
                                                   String.Join("\n   ", args) +"\n" +
                                                   "are not correct.\n \n" +
                                                   createHelpText(), ex);
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

        private static String parseDockerImageName(String dockerImage) 
        {
            // must be checked!
            // Regex expressions from:
            // https://github.com/docker/distribution/blob/master/reference/reference.go

            // String digestalgorithmseparator       = @"/[+.-_]/";
            // String digestalgorithmcomponent       = @"/[A-Za-z][A-Za-z0-9]*/";
            // String digesthex                      = @"/[0-9a-fA-F]{32,}/";
        
            // String digestalgorithm                = @digestalgorithmcomponent + "[" + digestalgorithmseparator + digestalgorithmcomponent + "]*";
            // String digest                         = @digestalgorithm + ":" + digesthex;

            // String tag                            = @"/[\w][\w.-]{0,127}/";  

            // String separator                      = @"/[_.]|__|[-]*/";
            // String alphanumeric                   = @"/[a-z0-9]+/";
            // String pathcomponent                  = @alphanumeric + "[" + separator + alphanumeric + "]*";
            // String portnumber                     = @"/[0-9]+/";
            // String domaincomponent                = @"/([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9-]*[a-zA-Z0-9])/";
            // String domain                         = @domaincomponent + "[." + domaincomponent + "]*[:" + portnumber + "]";
            // String name                           = @"[" + domain + "'/']" + pathcomponent + "['/'" + pathcomponent + "]*";
            // String reference                      = @name + "[ ':'" + tag + "] [ '@' " + digest + "]";

            // var result = Regex.Matches(dockerImage, digestalgorithmseparator);
            // result = Regex.Matches(dockerImage, digestalgorithmcomponent);
            // result = Regex.Matches(dockerImage, digesthex);
            // result = Regex.Matches(dockerImage, digestalgorithm);
            // result = Regex.Matches(dockerImage, digest);
            // result = Regex.Matches(dockerImage, tag);
            // result = Regex.Matches(dockerImage, separator);
            // result = Regex.Matches(dockerImage, alphanumeric);
            // result = Regex.Matches(dockerImage, pathcomponent);
            // result = Regex.Matches(dockerImage, portnumber);
            // result = Regex.Matches(dockerImage, domaincomponent);
            // result = Regex.Matches(dockerImage, domain);
            // result = Regex.Matches(dockerImage, name);
            // result = Regex.Matches(dockerImage, reference);

            // if (!Regex.IsMatch(dockerImage, pattern))
            //     throw new ArgumentException("The given docker image name\n" +
            //                                 "  " + dockerImage + "\n" +
            //                                 "is syntactically invalid."); 

            return dockerImage;
        }

        private static Uri parseDockerHostUri(String dockerHost) 
        {
            Uri result = null;
            try 
            {
                if (!Uri.TryCreate(dockerHost, UriKind.Absolute, out result))
                    throw new Exception();
                if (!result.IsWellFormedOriginalString())
                    throw new Exception();
            } catch (Exception fex) {
                throw new ArgumentException("Given docker host\n  "+ dockerHost +
                                            "\nis not a syntactically valid " +
                                            "and well-formed uri string.", fex);
            }
            return result;
        }

        private static String parseExecutionPath(String executionPath) 
        {
            try 
            {
                if ((!Path.IsPathRooted(executionPath)) && 
                    (!Path.IsPathFullyQualified(executionPath))) {
                        throw new Exception();
                }
            } catch (Exception) {
                throw new ArgumentException("Given execution path\n  "+ executionPath +
                                            "\nis not a syntactically valid path.");
            }
            return executionPath;
        }

        private static Version parseDockerRempoteApi(String dockerApi) 
        {
            Version result = null;
            try 
            {
                if (dockerApi != null) {
                    result = Version.Parse(dockerApi);
                }
            } catch (ArgumentOutOfRangeException orex) {
                throw new ArgumentException("At least one component of given docker api version\n  " +
                                            dockerApi + "\nis less than zero.", orex);                           
            } catch (ArgumentException aex) {
                throw new ArgumentException("Given version of docker api\n  " + dockerApi + "\n" +
                                            "has fewer than two or more than four components.", aex);
            } catch (FormatException fex) {
                throw new ArgumentException("At least one component of given docker api version\n  " +
                                            dockerApi + "\nis not an integer.", fex);
            } catch (OverflowException oex) {
                throw new ArgumentException("At least one component of given docker api version\n  " 
                                            + dockerApi +
                                            "\nrepresents a number that is greater than " + 
                                            Convert.ToString(Int32.MaxValue), oex);             
            }
            return result;
        }        

        private static String createHelpText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Usage:\t Investigator.UI");
            sb.Append("\n  -e, --experiment-data\t\t");
            sb.Append("json data of an series of experiments or path to json file, which contains the data\n");
            sb.Append("\t\t\t\tfile path must be escaped with an @ sign\n"); 
            sb.Append("\n  -d, --docker-image\t\t");
            sb.Append("name of docker image, which be used as object of the investigation\n");
            sb.Append("\t\t\t\tThe image name is structered as follows:\n");
            sb.Append("\t\t\t\t  [registry[:port]/]image-name[:tag]\n");
            sb.Append("\t\t\t\tIt must contains the following components:\n");
            sb.Append("\t\t\t\t  Experimenter software   to handle experiment data from Investigator\n");
            sb.Append("\t\t\t\t  ExecutorPlugin\t    to trigger the experiment software\n");
            sb.Append("\t\t\t\t  experiment software\t    as object of the investigation\n");
            sb.Append("\n  -h, --docker-host\t\t");
            sb.Append("host of docker, which will be connected");       
            sb.Append("\n  -p, --execution-path\t\t");
            sb.Append("path, which will be used to execute the experimenter program inside the given docker image");                 
            sb.Append("\n [-v, --docker-api-version]\t");
            sb.Append("remote api version, which will be used\n \n");
            sb.Append("Usage:\t Investigator.UI json-schema\n");
            sb.Append("\t Return the currently used json schema for the semantical validation.");            
            return sb.ToString();
        }            
    }
}
