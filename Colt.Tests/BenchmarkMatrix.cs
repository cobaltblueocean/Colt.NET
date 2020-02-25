// <copyright file="BenchmarkMatrix.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Cern.Colt;
using NUnit.Framework;
using Cern.Colt.Matrix;
using Cern.Colt.Matrix.Implementation;
using F = Cern.Hep.Aida.Bin.BinFunctions1D;

namespace Colt.Tests
{

    /// <summary>
    /// Configurable matrix benchmark.
    /// Runs the operations defined in main(args) or in the file specified by args.
    /// To get<a href="doc-files/usage.txt">this overall help</a> on usage type<i> java cern.colt.matrix.bench.BenchmarkMatrix -help</i>.
    /// To get help on usage of a given command, type<i> java cern.colt.matrix.bench.BenchmarkMatrix -help &lt; command&gt;</i>.
    /// Here is the<a href="doc-files/usage_dgemm.txt"> help ouput for the dgemm</a> command.
    /// <a href = "./doc-files/dgemmColt1.0.1ibm1.3LxPIII_2.txt" > Here </ a > is a sample result.
    /// For more results see the<a href="./doc-files/performanceLog.html"> performance log</a>.
    /// 
    /// @author wolfgang.hoschek @cern.ch
    /// @version 0.5, 10-May-2000
    /// </summary>
    [TestFixture]
    public class BenchmarkMatrix
    {
        private static StreamWriter writer;
        private static String path = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "\\TestResult\\BenchmarkMatrix\\";

        /// <summary>
        /// Runs the matrix benchmark operations defined in args or in the file specified by args0.
        /// To get detailed help on usage type java cern.colt.matrix.bench.BenchmarkMatrix -help
        /// <summary>
        [Test]
        public void MatrixBenchmarkTest()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            String[] args = Array.ConvertAll(commands().Split(','), p => p.Trim());

            String filename = path + "MatrixBenchmarkTest.txt";

            using (writer = new StreamWriter(filename))
            {

                int n = args.Length;
                if (n == 0 || (n <= 1 && args[0].Equals("-help")))
                { // overall help
                    writer.WriteLine(usage());
                    return;
                }
                if (args[0].Equals("-help"))
                { // help on specific command
                    if (commands().IndexOf(args[1]) < 0)
                    {
                        writer.WriteLine(args[1] + ": no such command available.\n" + usage());
                    }
                    else
                    {
                        writer.WriteLine(usage(args[1]));
                    }
                    return;
                }

                writer.WriteLine("Colt Matrix benchmark running on\n");
                writer.WriteLine(BenchmarkKernel.SystemInfo2() + "\n");


                string assemblyVersion = Assembly.Load("Cern.Colt").GetName().Version.ToString();

                writer.WriteLine("Colt Version is " + assemblyVersion + "\n");

                Timer timer = new Timer();
                timer.Start();
                if (!args[0].Equals("-file"))
                { // interactive mode, commands supplied via java class args
                    writer.WriteLine("\n\nExecuting command = [" + String.Join(",", args) + "] ..");
                    handle(args);
                }
                else
                { // batch mode, read commands from file
                  /* 
                  parse command file in args[0]
                  one command per line (including parameters)
                  for example:
                  // dgemm dense 2 2.0 false true 0.999 10 30 50 100 250 500 1000
                  dgemm dense 2 2.5 false true 0.999 10 50 
                  dgemm sparse 2 2.5 false true 0.001 500 1000  
                  */
                    StreamReader reader = null;
                    try
                    {
                        reader = new StreamReader(args[1]);
                    }
                    catch (IOException exc) { throw new SystemException(exc.Message); }

                    StreamTokenizer stream = new StreamTokenizer(reader);
                    //stream.eolIsSignificant(true);
                    //stream.SlashSlashComments(true); // allow // comments
                    //stream.SlashStarComments(true);  // allow /* comments */
                    try
                    {
                        var words = new List<Object>();
                        int token;
                        while ((token = stream.nextToken()) != StreamTokenizer.TT_EOF)
                        { // while not end of file
                            if (token == StreamTokenizer.TT_EOL)
                            { // execute a command line at a time
                              //writer.WriteLine(words);
                                if (words.Count > 0)
                                { // ignore emty lines
                                    String[] parameters = new String[words.Count];
                                    for (int i = 0; i < words.Count; i++) parameters[i] = (String)words[i];

                                    // execute command
                                    writer.WriteLine("\n\nExecuting command = " + words + " ..");
                                    handle(parameters);
                                }
                                words.Clear();
                            }
                            else
                            {
                                String word;
                                var formatter = new Cern.Colt.Matrix.Implementation.FormerFactory().Create("%G");
                                // ok: 2.0 -> 2   wrong: 2.0 -> 2.0 (kills int.Parse())
                                if (token == StreamTokenizer.TT_NUMBER)
                                    word = formatter.form(stream.Nval);
                                else
                                    word = stream.Sval;
                                if (word != null) words.Add(word);
                            }
                        }
                        reader.Close();

                        writer.WriteLine("\nCommand file name used: " + args[1] + "\nTo reproduce and compare results, here it's contents:");
                        try
                        {
                            reader = new StreamReader(args[1]);
                        }
                        catch (IOException exc) { throw new SystemException(exc.Message); }

                        /*InputStream input = new DataInputStream(new BufferedInputStream(new FileInputStream(args[1])));
                        BufferedReader d
                                       = new BufferedReader(new InputStreamReader(in));
                                       */
                        String line;
                        while ((line = reader.ReadLine()) != null)
                        { // while not end of file
                            writer.WriteLine(line);
                        }
                        reader.Close();

                    }
                    catch (IOException exc) { throw new SystemException(exc.Message); }
                }

                writer.WriteLine("\nProgram execution took a total of " + timer.Minutes() + " minutes.");
                writer.WriteLine("Good bye.");
            }
    }

        /// <summary>
        /// Not yet documented.
        /// <summary>
        protected static void bench_dgemm(String[] args)
        {
            String[] types;
            int cpus;
            double minSecs;
            Boolean transposeA;
            Boolean transposeB;
            double[] densities;
            int[] sizes;
            string filename = path + "bench_dgemm.xml";


            try
            { // parse
                int k = 1;
                types = new String[] { args[k++] };
                cpus = int.Parse(args[k++]);
                minSecs = Double.Parse(args[k++]);
                densities = new double[] { Double.Parse(args[k++]) };
                transposeA = Boolean.Parse(args[k++]);
                transposeB = Boolean.Parse(args[k++]);

                sizes = new int[args.Length - k];
                for (int i = 0; k < args.Length; k++, i++) sizes[i] = int.Parse(args[k]);
            }
            catch (Exception exc)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(args[0]);
                    writer.Write(exc.StackTrace);
                }

                //writer.WriteLine(usage(args[0]));
                //writer.WriteLine("Ignoring command..\n");
                return;
            }

            Cern.Colt.Matrix.LinearAlgebra.SmpBlas.AllocateBlas(cpus, Cern.Colt.Matrix.LinearAlgebra.SeqBlas.seqBlas);
            Double2DProcedure fun = fun_dgemm(transposeA, transposeB);
            String title = fun.ToString();
            String parameters = transposeA + ", " + transposeB + ", 1, A, B, 0, C";
            title = title + " dgemm(" + parameters + ")";
            run(minSecs, title, fun, types, sizes, densities);
        }

        /// <summary>
        /// Not yet documented.
        /// <summary>
        protected static void bench_dgemv(String[] args)
        {
            String[] types;
            int cpus;
            double minSecs;
            Boolean transposeA;
            double[] densities;
            int[] sizes;
            string filename = path + "bench_dgemv.xml";

            try
            { // parse
                int k = 1;
                types = new String[] { args[k++] };
                cpus = int.Parse(args[k++]);
                minSecs = Double.Parse(args[k++]);
                densities = new double[] { Double.Parse(args[k++]) };
                transposeA = Boolean.Parse(args[k++]);

                sizes = new int[args.Length - k];
                for (int i = 0; k < args.Length; k++, i++) sizes[i] = int.Parse(args[k]);
            }
            catch (Exception exc)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(args[0]);
                    writer.Write(exc.StackTrace);
                }

                //writer.WriteLine(usage(args[0]));
                //writer.WriteLine("Ignoring command..\n");
                return;
            }

            Cern.Colt.Matrix.LinearAlgebra.SmpBlas.AllocateBlas(cpus, Cern.Colt.Matrix.LinearAlgebra.SeqBlas.seqBlas);
            Double2DProcedure fun = fun_dgemv(transposeA);
            String title = fun.ToString();
            String parameters = transposeA + ", 1, A, B, 0, C";
            title = title + " dgemv(" + parameters + ")";
            run(minSecs, title, fun, types, sizes, densities);
        }
        /// <summary>
        /// Not yet documented.
        /// <summary>
        protected static void bench_pow(String[] args)
        {
            String[] types;
            int cpus;
            double minSecs;
            double[] densities;
            int exponent;
            int[] sizes;
            string filename = path + "bench_pow.xml";

            try
            { // parse
                int k = 1;
                types = new String[] { args[k++] };
                cpus = int.Parse(args[k++]);
                minSecs = Double.Parse(args[k++]);
                densities = new double[] { Double.Parse(args[k++]) };
                exponent = int.Parse(args[k++]);

                sizes = new int[args.Length - k];
                for (int i = 0; k < args.Length; k++, i++) sizes[i] = int.Parse(args[k]);
            }
            catch (Exception exc)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(args[0]);
                    writer.Write(exc.StackTrace);
                }

                //writer.WriteLine(usage(args[0]));
                //writer.WriteLine("Ignoring command..\n");
                return;
            }

            Cern.Colt.Matrix.LinearAlgebra.SmpBlas.AllocateBlas(cpus, Cern.Colt.Matrix.LinearAlgebra.SeqBlas.seqBlas);
            Double2DProcedure fun = fun_pow(exponent);
            String title = fun.ToString();
            String parameters = "A," + exponent;
            title = title + " pow(" + parameters + ")";
            run(minSecs, title, fun, types, sizes, densities);
        }
        /// <summary>
        /// Not yet documented.
        /// <summary>
        protected static void benchGeneric(Double2DProcedure fun, String[] args)
        {
            String[] types;
            int cpus;
            double minSecs;
            double[] densities;
            int[] sizes;
            string filename = path + "benchGeneric.xml";

            try
            { // parse
                int k = 1;
                types = new String[] { args[k++] };
                cpus = int.Parse(args[k++]);
                minSecs = Double.Parse(args[k++]);
                densities = new double[] { Double.Parse(args[k++]) };

                sizes = new int[args.Length - k];
                for (int i = 0; k < args.Length; k++, i++) sizes[i] = int.Parse(args[k]);
            }
            catch (Exception exc)
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(args[0]);
                    writer.Write(exc.StackTrace);
                }

                //writer.WriteLine(usage(args[0]));
                //writer.WriteLine("Ignoring command..\n");
                return;
            }

            Cern.Colt.Matrix.LinearAlgebra.SmpBlas.AllocateBlas(cpus, Cern.Colt.Matrix.LinearAlgebra.SeqBlas.seqBlas);
            String title = fun.ToString();
            run(minSecs, title, fun, types, sizes, densities);
        }
        /// <summary>
        /// 
        /// <summary>
        protected static String commands()
        {
            return "[dgemm, dense, 2, 2, 0.99, false, true, 5, 5, 50, 100, 300, 500, 1000]";
        }

        #region Test Class Constructors
        /// <summary>
        /// Linear algebrax matrix-matrix multiply.
        /// <summary>
        protected static Double2DProcedure fun_dgemm(Boolean transposeA, Boolean transposeB)
        {
            return new fun_dgemm_Double2DProcedure();
        }

        /// <summary>
        /// Linear algebrax matrix-matrix multiply.
        /// <summary>
        protected static Double2DProcedure fun_dgemv(Boolean transposeA)
        {
            return new fun_dgemv_Double2DProcedure();
        }

        /// <summary>
        /// 2D assign with get,set
        /// <summary>
        protected static Double2DProcedure fun_pow(int k)
        {
            return new fun_pow_Double2DProcedure(k);
        }

        /// <summary>
        /// 2D assign with A.Assign(B)
        /// <summary>
        protected static Double2DProcedure funAssign()
        {
            return new funAssign_Double2DProcedure();
        }

        /// <summary>
        /// 2D assign with get,set
        /// <summary>
        protected static Double2DProcedure funAssignGetSet()
        {
            return new funAssignGetSet_Double2DProcedure();

        }
        /// <summary>
        /// 2D assign with getQuick,setQuick
        /// <summary>
        protected static Double2DProcedure funAssignGetSetQuick()
        {
            return new funAssignGetSetQuick_Double2DProcedure();


        }
        /// <summary>
        /// 2D assign with A.Assign(B)
        /// <summary>
        protected static Double2DProcedure funAssignLog()
        {
            return new funAssignLog_Double2DProcedure();


        }
        /// <summary>
        /// 2D assign with A.Assign(B)
        /// <summary>
        protected static Double2DProcedure funAssignPlusMult()
        {
            return new funAssignPlusMult_Double2DProcedure();
        }
        /// <summary>
        /// Linear algebrax matrix-matrix multiply.
        /// <summary>
        protected static Double2DProcedure funCorrelation()
        {
            return new funCorrelation_Double2DProcedure();
        }
        /// <summary>
        /// Element-by-element matrix-matrix multiply.
        /// <summary>
        protected static Double2DProcedure funElementwiseMult()
        {
            return new funElementwiseMult_Double2DProcedure();
        }
        /// <summary>
        /// Element-by-element matrix-matrix multiply.
        /// <summary>
        protected static Double2DProcedure funElementwiseMultB()
        {
            return new funElementwiseMultB_Double2DProcedure();
        }
        /// <summary>
        /// 2D assign with get,set
        /// <summary>
        protected static Double2DProcedure funGetQuick()
        {
            return new funGetQuick_Double2DProcedure();
        }
        /// <summary>
        /// 2D assign with getQuick,setQuick
        /// <summary>
        protected static Double2DProcedure funLUDecompose()
        {
            return new funLUDecompose_Double2DProcedure();
        }
        /// <summary>
        /// 2D assign with getQuick,setQuick
        /// <summary>
        protected static Double2DProcedure funLUSolve()
        {
            return new funLUSolve_Double2DProcedure();
        }
        /// <summary>
        /// Linear algebrax matrix-matrix multiply.
        /// <summary>
        protected static Double2DProcedure funMatMultLarge()
        {
            return new funMatMultLarge_Double2DProcedure();
        }
        /// <summary>
        /// Linear algebrax matrix-vector multiply.
        /// <summary>
        protected static Double2DProcedure funMatVectorMult()
        {
            return new funMatVectorMult_Double2DProcedure();
        }
        /// <summary>
        /// 2D assign with get,set
        /// <summary>
        protected static Double2DProcedure funSetQuick()
        {
            return new funSetQuick_Double2DProcedure();
        }
        /// <summary>
        /// 
        /// <summary>
        protected static Double2DProcedure funSOR5()
        {
            return new funSOR5_Double2DProcedure();
        }
        /// <summary>
        /// 
        /// <summary>
        protected static Double2DProcedure funSOR8()
        {
            return new funSOR8_Double2DProcedure();
        }
        /// <summary>
        /// 
        /// <summary>
        protected static Double2DProcedure funSort()
        {
            return new funSort_Double2DProcedure();
        }
        #endregion

        /// <summary>
        /// Not yet documented.
        /// <summary>
        protected static DoubleFactory2D getFactory(String type)
        {
            DoubleFactory2D factory;
            if (type.Equals("dense")) return DoubleFactory2D.Dense;
            if (type.Equals("sparse")) return DoubleFactory2D.Sparse;
            if (type.Equals("rowCompressed")) return DoubleFactory2D.RowCompressed;
            String s = "type=" + type + " is unknownd Use one of {dense,sparse,rowCompressed}";
            throw new ArgumentException(s);
        }
        /// <summary>
        /// Not yet documented.
        /// <summary>
        protected static Double2DProcedure getGenericFunction(String cmd)
        {
            if (cmd.Equals("dgemm")) return fun_dgemm(false, false);
            else if (cmd.Equals("dgemv")) return fun_dgemv(false);
            else if (cmd.Equals("pow")) return fun_pow(2);
            else if (cmd.Equals("assign")) return funAssign();
            else if (cmd.Equals("assignGetSet")) return funAssignGetSet();
            else if (cmd.Equals("assignGetSetQuick")) return funAssignGetSetQuick();
            else if (cmd.Equals("elementwiseMult")) return funElementwiseMult();
            else if (cmd.Equals("elementwiseMultB")) return funElementwiseMultB();
            else if (cmd.Equals("SOR5")) return funSOR5();
            else if (cmd.Equals("SOR8")) return funSOR8();
            else if (cmd.Equals("LUDecompose")) return funLUDecompose();
            else if (cmd.Equals("LUSolve")) return funLUSolve();
            else if (cmd.Equals("assignLog")) return funAssignLog();
            else if (cmd.Equals("assignPlusMult")) return funAssignPlusMult();
            /*
            else if (cmd.Equals("xxxxxxxxxxxxxxxxx")) return xxxxx();
            }
            */
            return null;
        }
        /// <summary>
        /// Executes a command
        /// <summary>
        protected static Boolean handle(String[] parameters)
        {
            Boolean success = true;
            String cmd = parameters[0];
            if (cmd.Equals("dgemm")) bench_dgemm(parameters);
            else if (cmd.Equals("dgemv")) bench_dgemv(parameters);
            else if (cmd.Equals("pow")) bench_pow(parameters);
            else
            {
                Double2DProcedure fun = getGenericFunction(cmd);
                if (fun != null)
                {
                    benchGeneric(fun, parameters);
                }
                else
                {
                    success = false;
                    String s = "Command=" + parameters[0] + " is illegal or unknownd Should be one of " + commands() + "followed by appropriate parameters.\n" + usage() + "\nIgnoring this line.\n";
                    writer.WriteLine(s);
                }
            }
            return success;
        }

        /// <summary>
        /// Executes procedure repeatadly until more than minSeconds have elapsed.
        /// <summary>
        protected static void run(double minSeconds, String title, Double2DProcedure function, String[] types, int[] sizes, double[] densities)
        {
            //int[] sizes = {33,500,1000};
            //double[] densities = {0.001,0.01,0.99};

            //int[] sizes = {3,5,7,9,30,45,60,61,100,200,300,500,800,1000};
            //double[] densities = {0.001,0.01,0.1,0.999};

            //int[] sizes = {3};
            //double[] densities = {0.1};

            DoubleMatrix3D timings = DoubleFactory3D.Dense.Make(types.Length, sizes.Length, densities.Length);
            Timer runTime = new Timer();
            runTime.Start();
            for (int k = 0; k < types.Length; k++)
            {
                //DoubleFactory2D factory = (k==0 ? DoubleFactory2D.Dense : k==1 ? DoubleFactory2D.Sparse : DoubleFactory2D.rowCompressed);
                //DoubleFactory2D factory = (k==0 ? DoubleFactory2D.Dense : k==1 ? DoubleFactory2D.Sparse : k==2 ? DoubleFactory2D.rowCompressed : DoubleFactory2D.rowCompressedModified);
                DoubleFactory2D factory = getFactory(types[k]);
                Assert.Inconclusive("\n@");

                for (int i = 0; i < sizes.Length; i++)
                {
                    int size = sizes[i];
                    Assert.Inconclusive("x");
                    //writer.WriteLine("doing size="+size+"..");

                    for (int j = 0; j < densities.Length; j++)
                    {
                        double density = densities[j];
                        Assert.Inconclusive(".");
                        //writer.WriteLine("   doing density="+density+"..");
                        float opsPerSec;

                        //if (true) {
                        //if (!((k==1 && density >= 0.1 && size >=100) || (size>5000 && (k==0 || density>1.0E-4) ))) {
                        if (!((k > 0 && density >= 0.1 && size >= 500)))
                        {
                            double val = 0.5;
                            function.A = null; function.B = null; function.C = null; function.D = null; // --> help gc before allocating new mem
                            DoubleMatrix2D A = factory.Sample(size, size, val, density);
                            DoubleMatrix2D B = factory.Sample(size, size, val, density);
                            function.SetParameters(A, B);
                            A = null; B = null; // help gc
                            double ops = function.Operations();
                            double secs = BenchmarkKernel.Run(minSeconds, function.timerProc);
                            opsPerSec = (float)(ops / secs);
                        }
                        else
                        { // skip this parameter combination (not used in practice & would take a lot of memory and time)
                            opsPerSec = float.NaN;
                        }
                        timings[k, i, j] = opsPerSec;
                        //writer.WriteLine(secs);
                        //writer.WriteLine(opsPerSec+" Mops/sec\n");
                    }
                }
            }
            runTime.Stop();

            String sliceAxisName = "type";
            String rowAxisName = "size";
            String colAxisName = "d"; //"density";
                                      //String[] sliceNames = {"dense", "sparse"};
                                      //String[] sliceNames = {"dense", "sparse", "rowCompressed"};
            String[] sliceNames = types;
            Cern.Hep.Aida.Bin.BinFunction1D[] aggr = null; //{F.mean, F.median, F.Sum};
            String[] rowNames = new String[sizes.Length];
            String[] colNames = new String[densities.Length];
            for (int i = sizes.Length; --i >= 0;) rowNames[i] = sizes[i].ToString();
            for (int j = densities.Length; --j >= 0;) colNames[j] = densities[j].ToString();
            writer.WriteLine("*");
            // show transposed
            String tmp = rowAxisName; rowAxisName = colAxisName; colAxisName = tmp;
            String[] tmp2 = rowNames; rowNames = colNames; colNames = tmp2;
            timings = timings.ViewDice(0, 2, 1);
            writer.WriteLine(new Cern.Colt.Matrix.DoubleAlgorithms.Formatter("%1.3G").ToTitleString(timings, sliceNames, rowNames, colNames, sliceAxisName, rowAxisName, colAxisName, "Performance of " + title, aggr));
            /*
            title = "Speedup of dense over sparse";
            DoubleMatrix2D speedup = cern.colt.matrix.doublealgo.Transform.div(timings.viewSlice(0).Copy(),timings.viewSlice(1));
            writer.WriteLine("\n"+new cern.colt.matrix.doublealgo.Formatter("%1.3G").toTitleString(speedup,rowNames,colNames,rowAxisName,colAxisName,title,aggr));
            */
            writer.WriteLine("Run took a total of " + runTime + "d End of run.");
        }
        /// <summary>
        /// Executes procedure repeatadly until more than minSeconds have elapsed.
        /// <summary>
        protected static void runSpecial(double minSeconds, String title, Double2DProcedure function)
        {
            int[] sizes = { 10000 };
            double[] densities = { 0.00001 };
            Boolean[] sparses = { true };

            DoubleMatrix2D timings = DoubleFactory2D.Dense.Make(sizes.Length, 4);
            Timer runTime = new Timer();
            runTime.Start();
            for (int i = 0; i < sizes.Length; i++)
            {
                int size = sizes[i];
                double density = densities[i];
                Boolean sparse = sparses[i];
                DoubleFactory2D factory = (sparse ? DoubleFactory2D.Sparse : DoubleFactory2D.Dense);
                Assert.Inconclusive("\n@");

                Assert.Inconclusive("x");
                double val = 0.5;
                function.A = null; function.B = null; function.C = null; function.D = null; // --> help gc before allocating new mem
                DoubleMatrix2D A = factory.Sample(size, size, val, density);
                DoubleMatrix2D B = factory.Sample(size, size, val, density);
                function.SetParameters(A, B);
                A = null; B = null; // help gc
                float secs = BenchmarkKernel.Run(minSeconds, function.timerProc);
                double ops = function.Operations();
                float opsPerSec = (float)(ops / secs);
                timings.ViewRow(i)[0] = sparse ? 0 : 1;
                timings.ViewRow(i)[1] = size;
                timings.ViewRow(i)[2] = density;
                timings.ViewRow(i)[3] = opsPerSec;
                //writer.WriteLine(secs);
                //writer.WriteLine(opsPerSec+" Mops/sec\n");
            }
            runTime.Stop();

            //Cern.Hep.Aida.Bin.BinFunctions1D F = Cern.Hep.Aida.Bin.BinFunctions1D.functions;
            Cern.Hep.Aida.Bin.BinFunction1D[] aggr = null; //{F.mean, F.median, F.Sum};
            String[] rowNames = null;
            String[] colNames = { "dense (y=1,n=0)", "size", "density", "flops/sec" };
            String rowAxisName = null;
            String colAxisName = null;
            writer.WriteLine("*");
            writer.WriteLine(new Cern.Colt.Matrix.DoubleAlgorithms.Formatter("%1.3G").ToTitleString(timings, rowNames, colNames, rowAxisName, colAxisName, title, aggr));

            writer.WriteLine("Run took a total of " + runTime + "d End of run.");
        }
        /// <summary>
        /// Overall usage.
        /// <summary>
        protected static String usage()
        {
            String usage =
        "\nUsage (help): To get this help, type java cern.colt.matrix.bench.BenchmarkMatrix -help\n" +
        "To get help on a command's args, omit args and type java cern.colt.matrix.bench.BenchmarkMatrix -help <command>\n" +
        "Available commands: " + commands() + "\n\n" +

        "Usage (direct): java cern.colt.matrix.bench.BenchmarkMatrix command {args}\n" +
        "Example: dgemm dense 2 2.0 0.999 false true 5 10 25 50 100 250 500\n\n" +

        "Usage (batch mode): java cern.colt.matrix.bench.BenchmarkMatrix -file <file>\nwhere <file> is a text file with each line holding a command followed by appropriate args (comments and empty lines ignored).\n\n" +
        "Example file's content:\n" +
        "dgemm dense 1 2.0 0.999 false true 5 10 25 50 100 250 500\n" +
        "dgemm dense 2 2.0 0.999 false true 5 10 25 50 100 250 500\n\n" +
        "/*\n" +
        "Java like comments in file are ignored\n" +
        "dgemv dense 1 2.0 0.001 false 5 10 25 50 100 250 500 1000\n" +
        "dgemv sparse 1 2.0 0.001 false 5 10 25 50 100 250 500 1000\n" +
        "dgemv rowCompressed 1 2.0 0.001 false 5 10 25 50 100 250 500 1000\n" +
        "*/\n" +
        "// more comments ignored\n";
            return usage;
        }
        /// <summary>
        /// Usage of a specific command.
        /// <summary>
        protected static String usage(String cmd)
        {
            String usage = cmd + " description: " + getGenericFunction(cmd).ToString() +
            "\nArguments to be supplied:\n" +
                //String usage = "Illegal arguments! Arguments to be supplied:\n" +
                //"\te.gd "+cmd+" dense 2 2.0 false 0.999 10 30 50 100 250 500 1000\n"+
                "\t<operation> <type> <cpus> <minSecs> <density>";
            if (cmd.Equals("dgemv")) usage = usage + " <transposeA>";
            if (cmd.Equals("dgemm")) usage = usage + " <transposeA> <transposeB>";
            if (cmd.Equals("pow")) usage = usage + " <exponent>";
            usage = usage +
                " {sizes}\n" +
                "where\n" +
                "\toperation = the operation to benchmark; in this case: " + cmd + "\n" +
                "\ttype = matrix type to be used; e.gd dense, sparse or rowCompressed\n" +
                "\tcpus = #cpus available; e.gd 1 or 2 or ..\n" +
                "\tminSecs = #seconds each operation shall at least run; e.gd 2.0 is a good number giving realistic timings\n" +
                "\tdensity = the density of the matrices to be benchmarked; e.gd 0.999 is very dense, 0.001 is very sparse\n";

            if (cmd.Equals("dgemv")) usage = usage + "\ttransposeA = false or true\n";
            if (cmd.Equals("dgemm")) usage = usage + "\ttransposeA = false or true\n\ttransposeB = false or true\n";
            if (cmd.Equals("pow")) usage = usage + "\texponent = the number of times to multiply; e.gd 1000\n";
            usage = usage +
                "\tsizes = a list of problem sizes; e.gd 100 200 benchmarks squared 100x100 and 200x200 matrices";
            return usage;
        }

        #region Private Test Classes
        private class fun_dgemm_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public override String ToString() { return "Blas matrix-matrix mult"; }

            public fun_dgemm_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) => { Cern.Colt.Matrix.LinearAlgebra.SmpBlas.smpBlas.Dgemm(TransposeA, TransposeB, 1, A, B, 0, C); });
            }

            public override void SetParameters(DoubleMatrix2D G, DoubleMatrix2D H)
            {
                base.SetParameters(G, H);
                D = new Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D(A.Rows, A.Columns).Assign(0.5);
                C = D.Copy();
                B = D.Copy();
            }
            public override void Init() { C.Assign(D); }

            public override double Operations()
            { // Mflops
                double m = A.Rows;
                double n = A.Columns;
                double p = B.Columns;
                return 2.0 * m * n * p / 1.0E6;
            }
        }

        private class fun_dgemv_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public fun_dgemv_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) => { Cern.Colt.Matrix.LinearAlgebra.SmpBlas.smpBlas.Dgemv(TransposeA, 1, A, B.ViewRow(0), 0, C.ViewRow(0)); });

            }

            public override void SetParameters(DoubleMatrix2D G, DoubleMatrix2D H)

            {
                base.SetParameters(G, H);
                D = new Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D(A.Rows, A.Columns).Assign(0.5);
                C = D.Copy();
                B = D.Copy();
            }

            public override double Operations()
            { // Mflops
                double m = A.Rows;
                double n = A.Columns;
                //double p = B.Columns;
                return 2.0 * m * n / 1.0E6;
            }


            public override void Init()
            {
                C.ViewRow(0).Assign(D.ViewRow(0));
            }

            public override String ToString() { return "Blas matrix-vector mult"; }
        }

        private class fun_pow_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }
            public int k { get; set; }

            public fun_pow_Double2DProcedure(int _k)
            {
                k = _k;
                timerProc = new TimerProcedure((t) => { Cern.Colt.Matrix.LinearAlgebra.Algebra.Pow(A, k); });
            }

            public override void Init()
            {

            }

            public double dummy;
            public override String ToString() { return "matrix to the power of an exponent"; }
            public override void SetParameters(DoubleMatrix2D A, DoubleMatrix2D B)
            {
                if (k < 0)
                { // must be nonsingular for inversion
                    if (!Cern.Colt.Matrix.LinearAlgebra.Property.ZERO.isDiagonallyDominantByRow(A) ||
                        !Cern.Colt.Matrix.LinearAlgebra.Property.ZERO.isDiagonallyDominantByColumn(A))
                    {
                        Cern.Colt.Matrix.LinearAlgebra.Property.ZERO.generateNonSingular(A);
                    }
                    base.SetParameters(A, B);
                }
            }

            public override double Operations()
            { // Mflops
                double m = A.Rows;
                if (k == 0) return m; // identity
                double mflops = 0;
                if (k < 0)
                {
                    // LU.decompose
                    double N = System.Math.Min(A.Rows, A.Columns);
                    mflops += (2.0 * N * N * N / 3.0 / 1.0E6);

                    // LU.Solve
                    double n = A.Columns;
                    double nx = B.Columns;
                    mflops += (2.0 * nx * (n * n + n) / 1.0E6);
                }
                // mult
                mflops += 2.0 * (System.Math.Abs(k) - 1) * m * m * m / 1.0E6;
                return mflops;
            }
        }

        private class funAssign_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funAssign_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) => { A.Assign(B); });

            }

            public override void Init()
            {
                A.Assign(0);
            }

            public override String ToString() { return "A.Assign(B) [Mops/sec]"; }
        }

        private class funAssignGetSet_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funAssignGetSet_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    int rows = B.Rows;
                    int columns = B.Columns;
                    /*
                    for (int row=rows; --row >= 0; ) {
                        for (int column=columns; --column >= 0; ) {
                            A.Set(row,column, B.Get(row,column));
                        }
                    }
                    */
                    for (int row = 0; row < rows; row++)
                    {
                        for (int column = 0; column < columns; column++)
                        {
                            A[row, column] = B[row, column];
                        }
                    }
                });

            }

            public override void Init()
            {
                A.Assign(0);
            }

            public override String ToString() { return "A.Assign(B) via get and set [Mops/sec]"; }

        }

        private class funAssignGetSetQuick_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funAssignGetSetQuick_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    int rows = B.Rows;
                    int columns = B.Columns;
                    //for (int row=rows; --row >= 0; ) {
                    //	for (int column=columns; --column >= 0; ) {
                    for (int row = 0; row < rows; row++)
                    {
                        for (int column = 0; column < columns; column++)
                        {
                            A[row, column] = B[row, column];
                        }
                    }
                });

            }

            public override String ToString() { return "A.Assign(B) via getQuick and setQuick [Mops/sec]"; }
            public override void Init()
            {
                A.Assign(0);
            }

        }

        private class funAssignLog_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funAssignLog_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    Cern.Colt.Matrix.LinearAlgebra.SmpBlas.smpBlas.Assign(A, Cern.Jet.Math.Functions.DoubleFunctions.Log);
                });

            }

            public override void Init()
            {
                A.Assign(C);
            }

            public override String ToString() { return "A[i,j] = log(A[i,j]) via Blas.Assign(fun) [Mflops/sec]"; }

        }

        private class funAssignPlusMult_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funAssignPlusMult_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    Cern.Colt.Matrix.LinearAlgebra.SmpBlas.smpBlas.Assign(A, B, Cern.Jet.Math.Functions.DoubleDoubleFunctions.PlusMult(0.5));
                });

            }

            public override void Init()
            {
                A.Assign(C);
            }

            public override double Operations()
            { // Mflops
                double m = A.Rows;
                double n = A.Columns;
                return 2 * m * n / 1.0E6;
            }

            public override String ToString() { return "A[i,j] = A[i,j] + s*B[i,j] via Blas.Assign(fun) [Mflops/sec]"; }
        }

        private class funCorrelation_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funCorrelation_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    Cern.Colt.Matrix.DoubleAlgorithms.Statistics.Correlation(
                        Cern.Colt.Matrix.DoubleAlgorithms.Statistics.Covariance(A));

                });

            }

            public override void Init()
            {

            }

            public override void SetParameters(DoubleMatrix2D A, DoubleMatrix2D B)
            {
                base.SetParameters(A.ViewDice(), B); // transposed --> faster (memory aware) iteration in correlation algo
            }

            public override double Operations()
            { // Mflops
                double m = A.Rows;
                double n = A.Columns;
                return m * (n * n + n) / 1.0E6;
            }

            public override String ToString() { return "xxxxxxx"; }
        }

        private class funElementwiseMult_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funElementwiseMult_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) => { Cern.Colt.Matrix.LinearAlgebra.SmpBlas.smpBlas.Assign(A, Cern.Jet.Math.Functions.DoubleFunctions.Mult(0.5)); });

            }

            public override void Init()
            {
                A.Assign(C);
            }

            public override String ToString() { return "A.Assign(F.mult(0.5)) via Blas [Mflops/sec]"; }
        }

        private class funElementwiseMultB_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funElementwiseMultB_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) => { Cern.Colt.Matrix.LinearAlgebra.SmpBlas.smpBlas.Assign(A, B, Cern.Jet.Math.Functions.DoubleDoubleFunctions.Mult); });

            }

            public override void Init()
            {
                A.Assign(C);
            }

            public override String ToString() { return "A.Assign(B,F.mult) via Blas [Mflops/sec]"; }
        }

        private class funGetQuick_Double2DProcedure : Double2DProcedure
        {
            public double dummy;
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funGetQuick_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    int rows = B.Rows;
                    int columns = B.Columns;
                    double sum = 0;
                    //for (int row=rows; --row >= 0; ) {
                    //	for (int column=columns; --column >= 0; ) {
                    for (int row = 0; row < rows; row++)
                    {
                        for (int column = 0; column < columns; column++)
                        {
                            sum += A[row, column];
                        }
                    }
                    dummy = sum;

                });

            }

            public override void Init()
            {

            }

            public override String ToString() { return "xxxxxxx"; }
        }

        private class funLUDecompose_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }
            Cern.Colt.Matrix.LinearAlgebra.LUDecompositionQuick lu = new Cern.Colt.Matrix.LinearAlgebra.LUDecompositionQuick(0);

            public funLUDecompose_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    lu.Decompose(A);
                });

            }

            public override void Init()
            {
                A.Assign(C);
            }

            public override double Operations()
            { // Mflops
                double N = System.Math.Min(A.Rows, A.Columns);
                return (2.0 * N * N * N / 3.0 / 1.0E6);
            }

            public override string ToString() { return "lu.decompose(a) [mflops/sec]"; }
        }

        private class funLUSolve_Double2DProcedure : Double2DProcedure
        {

            Cern.Colt.Matrix.LinearAlgebra.LUDecompositionQuick lu;
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funLUSolve_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    lu.Solve(B);
                });

            }

            public override void Init()
            {
                B.Assign(D);
            }

            public override String ToString() { return "LU.Solve(A) [Mflops/sec]"; }

            public override void SetParameters(DoubleMatrix2D A, DoubleMatrix2D B)
            {
                lu = null;
                if (!Cern.Colt.Matrix.LinearAlgebra.Property.ZERO.isDiagonallyDominantByRow(A) ||
                    !Cern.Colt.Matrix.LinearAlgebra.Property.ZERO.isDiagonallyDominantByColumn(A))
                {
                    Cern.Colt.Matrix.LinearAlgebra.Property.ZERO.generateNonSingular(A);
                }
                base.SetParameters(A, B);
                lu = new Cern.Colt.Matrix.LinearAlgebra.LUDecompositionQuick(0);
                lu.Decompose(A);
            }

            public override double Operations()
            { // Mflops
                double n = A.Columns;
                double nx = B.Columns;
                return (2.0 * nx * (n * n + n) / 1.0E6);
            }
        }

        private class funMatMultLarge_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funMatMultLarge_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    A.ZMult(B, C);
                });

            }

            public override void Init()
            {
                C.Assign(0);
            }

            public override void SetParameters(DoubleMatrix2D A, DoubleMatrix2D B)
            {
                // do not allocate mem for "D" --> safe some mem
                this.A = A;
                this.B = B;
                this.C = A.Copy();
            }

            public override double Operations()
            { // Mflops
                double m = A.Rows;
                double n = A.Columns;
                double p = B.Columns;
                return 2.0 * m * n * p / 1.0E6;
            }

            public override String ToString() { return "xxxxxxx"; }
        }

        private class funMatVectorMult_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funMatVectorMult_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    A.ZMult(B.ViewRow(0), C.ViewRow(0));
                });

            }

            public override void Init()
            {
                C.ViewRow(0).Assign(D.ViewRow(0));
            }

            public override void SetParameters(DoubleMatrix2D G, DoubleMatrix2D H)
            {
                base.SetParameters(G, H);
                D = new Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D(A.Rows, A.Columns).Assign(0.5);
                C = D.Copy();
                B = D.Copy();
            }

            public override double Operations()
            { // Mflops
                double m = A.Rows;
                double n = A.Columns;
                //double p = B.Columns;
                return 2.0 * m * n / 1.0E6;
            }

            public override String ToString() { return "xxxxxxx"; }
        }

        private class funSetQuick_Double2DProcedure : Double2DProcedure
        {
            private int current;
            private double density;
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funSetQuick_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    int rows = B.Rows;
                    int columns = B.Columns;
                    //for (int row=rows; --row >= 0; ) {
                    //	for (int column=columns; --column >= 0; ) {
                    for (int row = 0; row < rows; row++)
                    {
                        for (int column = 0; column < columns; column++)
                        {
                            // a very fast random number generator (this is an inline version of class cern.jet.random.engine.DRand)
                            current *= 0x278DDE6D;
                            double random = (double)(current & 0xFFFFFFFFL) * 2.3283064365386963E-10;
                            // random uniform in (0.0,1.0)
                            if (random < density)
                                A[row, column] = random;
                            else
                                A[row, column] = 0;
                        }
                    }

                });

            }

            public override void Init()
            {
                A.Assign(0);
                int seed = 123456;
                current = 4 * seed + 1;
                density = A.Cardinality() / (double)A.Size;
            }

            public override String ToString() { return "xxxxxxx"; }
        }

        private class funSOR5_Double2DProcedure : Double2DProcedure
        {
            public double value = 2;
            public double omega = 1.25;
            public double alpha = 1.25 * 0.25;
            public double beta = 1 - 1.25;
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }
            Cern.Colt.Function.Double9Function function;

            public funSOR5_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) => { A.ZAssign8Neighbors(B, function); });
                function = new Cern.Colt.Function.Double9Function((
                            double a00, double a01, double a02,
                            double a10, double a11, double a12,
                            double a20, double a21, double a22) =>
                {
                    return alpha * a11 + beta * (a01 + a10 + a12 + a21);
                });
            }

            public override void Init()
            {
                B.Assign(D);
            }

            public override String ToString() { return "A.zAssign8Neighbors(5 point function) [Mflops/sec]"; }

            public override double Operations()
            { // Mflops
                double n = A.Columns;
                double m = A.Rows;
                return 6.0 * m * n / 1.0E6;
            }
        }

        private class funSOR8_Double2DProcedure : Double2DProcedure
        {
            double value = 2;
            double omega = 1.25;
            double alpha = 1.25 * 0.25;
            double beta = 1 - 1.25;
            Cern.Colt.Function.Double9Function function;
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funSOR8_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) =>
                {
                    A.ZAssign8Neighbors(B, function);
                });

                function = new Cern.Colt.Function.Double9Function((
                                double a00, double a01, double a02,
                                double a10, double a11, double a12,
                                double a20, double a21, double a22) =>
                {
                    return alpha * a11 + beta * (a00 + a10 + a20 + a01 + a21 + a02 + a12 + a22);
                });
            }

            public override void Init()
            {
                B.Assign(D);
            }

            public override double Operations()
            { // Mflops
                double n = A.Columns;
                double m = A.Rows;
                return 10.0 * m * n / 1.0E6;
            }

            public override String ToString() { return "A.ZAssign8Neighbors(9 point function) [Mflops/sec]"; }
        }

        private class funSort_Double2DProcedure : Double2DProcedure
        {
            public Boolean TransposeA { get; set; }
            public Boolean TransposeB { get; set; }

            public funSort_Double2DProcedure()
            {
                timerProc = new TimerProcedure((t) => { A.ViewSorted(0); });

            }

            public override void Init()
            {
                A.Assign(C);
            }

            public override String ToString() { return "xxxxxxx"; }
        }
        #endregion

    }
}
