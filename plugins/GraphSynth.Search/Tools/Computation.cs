using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace GraphSynth.Search.Tools
{
    public class Computation
    {
        private readonly string _dataDir;
        private readonly string _learnDir;
        private readonly string _featureDir;
        private readonly string _propertyDir;
        private readonly string _featureUsed;
        private readonly string _propertyUsed;


        private static readonly Dictionary<string, string> featureScriptLookup = new Dictionary<string, string>()
        {
            {"point", "calcPoint.py"}
        };
        private static readonly Dictionary<string, string> propertyScriptLookup = new Dictionary<string, string>()
        {
            {"stiff", "calcStiff.py"}
        };

        public Computation(string runDir, string learnDir, string feature, string property)
        {
            _dataDir = Path.Combine(_runDir, "data");
            _learnDir = learnDir;
            _featureUsed = feature;
            _propertyUsed = property;
            _featureDir = Path.Combine(_runDir, "feature", feature);
            _propertyDir = Path.Combine(_runDir, "property", property);


            if (Directory.Exists(_featureDir))
                Directory.Delete(_featureDir, true);
            Directory.CreateDirectory(_featureDir);

            if (Directory.Exists(_propertyDir))
                Directory.Delete(_propertyDir, true);
            Directory.CreateDirectory(_propertyDir);

        }


        public void CalculateFeature(string linkerId, bool isPossible)
        {
            var actualDataDir = _dataDir;
            var actualFeatureDir = _featureDir;
            if (isPossible)
            {
                actualDataDir = Path.Combine(actualDataDir, "possible");
                actualFeatureDir = Path.Combine(actualFeatureDir, "possible");
            }

            var lmpData = Path.Combine(actualDataDir, "linker" + linkerId + ".lmpdat");
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "/rhome/yangchen/.conda/envs/yangchenPython3/bin/python";
                proc.StartInfo.Arguments = featureScriptLookup[_featureUsed] + " " + lmpData + " " + actualFeatureDir;
                proc.StartInfo.WorkingDirectory = Path.Combine(_learnDir, "computation");
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardInput = false;
                proc.Start();
                proc.WaitForExit();
                //string output = proc.StandardOutput.ReadToEnd();
                //string error = proc.StandardError.ReadToEnd();
                //Console.WriteLine(error);
                //Console.WriteLine(output);
            }
        }

        public string CalculateProperty(string linkerId)
        {
            var aveData = Path.Combine(_dataDir, "linker" + linkerId + "_deformation", "linker" + linkerId + "-ave-force.d");
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "/rhome/yangchen/.conda/envs/yangchenPython3/bin/python";
                proc.StartInfo.Arguments = propertyScriptLookup[_propertyUsed] + " " + aveData + " " + _propertyDir;
                proc.StartInfo.WorkingDirectory = Path.Combine(_learnDir, "computation");
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardInput = false;
                proc.Start();
                proc.WaitForExit();
                //string error = proc.StandardError.ReadToEnd();
                //Console.WriteLine(error);
                string output = proc.StandardOutput.ReadToEnd();
                //Console.WriteLine(output);
                return output;

            }
        }
    }


}