﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotnetCampus.GitCommand;

namespace CopyAfterCompile
{
    /// <summary>
    /// 对二分查找做准备，编译每个提交
    /// </summary>
    class BinaryChopCompiler
    {
        /// <inheritdoc />
        public BinaryChopCompiler(DirectoryInfo codeDirectory,
            DirectoryInfo targetDirectory,
            DirectoryInfo outputDirectory = null,
            string originBranch = null,
            ICompiler compiler = null)
        {
            CodeDirectory = codeDirectory;
            TargetDirectory = targetDirectory;

            if (!string.IsNullOrEmpty(originBranch))
            {
                OriginBranch = originBranch;
            }

            var git = new Git(codeDirectory);

            _git = git;
            _lastCommit = ReadLastCommit();

            Compiler = compiler ?? new Compiler();

            if (outputDirectory is null)
            {
                outputDirectory = new DirectoryInfo(Path.Combine(codeDirectory.FullName, "bin"));
            }

            OutputDirectory = outputDirectory;
        }

        private string ReadLastCommit()
        {
            if (System.IO.File.Exists(LastCommitFile))
            {
                return System.IO.File.ReadAllText(LastCommitFile);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 存放最后一次提交
        /// </summary>
        private const string LastCommitFile = "last commit.txt";

        private string _lastCommit;
        private readonly Git _git;

        public string OriginBranch { get; } = "dev";

        private ICompiler Compiler { get; }

        /// <summary>
        /// 移动到的文件夹，编译完成将输出移动到这个文件夹
        /// </summary>
        public DirectoryInfo TargetDirectory { get; }

        public DirectoryInfo CodeDirectory { get; }

        /// <summary>
        /// 输出文件夹
        /// </summary>
        public DirectoryInfo OutputDirectory { get; }

        public void CompileAllCommitAndCopy()
        {
            var commitList = GetCommitList().Reverse().ToList();

            foreach (var commit in commitList)
            {
                Console.WriteLine($"开始 {commit} 二分");
                CleanDirectory(commit);

                Compiler.Compile();
                MoveFile(commit);

                SaveLastCommit(commit);
            }
        }

        private void SaveLastCommit(string commit)
        {
            _lastCommit = commit;
            System.IO.File.WriteAllText(LastCommitFile, commit);
        }

        private void MoveFile(string commit)
        {
            var outputDirectory = new DirectoryInfo(OutputDirectory.FullName);

            var moveDirectory = Path.Combine(TargetDirectory.FullName, commit);
            Console.WriteLine($"将{outputDirectory.FullName}移动到{moveDirectory}");

            outputDirectory.MoveTo(moveDirectory);
        }

        private void CleanDirectory(string commit)
        {
            Console.WriteLine($"开始清空仓库");

            var git = _git;
            git.Clean();
            git.Checkout(commit);
        }

        private string[] GetCommitList()
        {
            var git = _git;
            if (_lastCommit is null)
            {
                return git.GetLogCommit();
            }
            else
            {
                return git.GetLogCommit(_lastCommit, OriginBranch);
            }
        }
    }
}