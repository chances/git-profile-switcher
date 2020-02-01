using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using GitProfileSwitcher.Logs;

namespace GitProfileSwitcher.Tasks
{
    public class GitConfigTask
    {
        private const string _gitCommand = "/usr/bin/git"; // TODO: Use /usr/bin/env to get the path to Git

        public const string UserName = "user.name";
        public const string UserEmail = "user.email";

        public static async Task<bool> SetAndConfirm(string property, string value)
        {
            try
            {
                var configArguments = new string[] { "config", "--global", property };

                var gitConfigTask = await LaunchTask(
                    _gitCommand,
                    configArguments.Append(value).ToArray());
                var statusCode = gitConfigTask.TerminationStatus;
                if (statusCode != 0)
                {
                    Logging.Trace($"Non-zero exit code: {statusCode}", new { statusCode });
                    return false;
                }

                // Confirm changes were made
                gitConfigTask = await LaunchTask(_gitCommand, configArguments, captureStdOut: true);
                statusCode = gitConfigTask.TerminationStatus;
                if (statusCode != 0)
                {
                    Logging.Trace($"Non-zero exit code: {statusCode}", new { statusCode });
                    return false;
                }

                return gitConfigTask.StandardOutput.StartsWith(value);
            }
            catch (Exception e)
            {
                Logging.Exception(e, new { property });

                return false;
            }
        }

        private static Task<TaskOutput> LaunchTask(string launchPath, string[] arguments, bool captureStdOut = false)
        {
            var taskTerminatedSource = new TaskCompletionSource<TaskOutput>();

            var pipe = new NSPipe();
            var stdOut = pipe.ReadHandle;
            NSTask task = null;

            try
            {
                task = new NSTask() {
                    LaunchPath = launchPath,
                    Arguments = arguments
                };
                if (captureStdOut)
                {
                    task.StandardOutput = pipe;
                }
                task.Launch();
                task.WaitUntilExit();

                string result = null;
                if (captureStdOut)
                {
                    using var stdOutReader = new StreamReader(
                        stdOut.ReadDataToEndOfFile().AsStream(), Encoding.UTF8);
                    result = stdOutReader.ReadToEnd();
                    stdOut.CloseFile();
                }

                taskTerminatedSource.SetResult(new TaskOutput() {
                    TerminationStatus = task.TerminationStatus,
                    StandardOutput = result
                });
            }
            catch (Exception e)
            {
                Logging.Trace(e, new { launchPath });
                taskTerminatedSource.SetException(e);
            }
            finally
            {
                task?.Terminate();
                task?.Dispose();
            }

            return taskTerminatedSource.Task;
        }
    }
}
