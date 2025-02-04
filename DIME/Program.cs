﻿using Topshelf;
using Topshelf.Logging;
using Topshelf.Runtime;

namespace DIME;

public static class Program
{
    public static void Main(string[] args)
    {
        HostFactory.Run(x =>
        {
            x.Service<TransporterService>(s =>
            {
                s.ConstructUsing(name => new TransporterService(new FilesystemYamlConfigurationProvider()));
                s.WhenStarted(tc => tc.Start());
                s.WhenStopped(tc => tc.Stop());
            });
            x.RunAsLocalSystem();
            x.EnableHandleCtrlBreak();
            x.SetServiceName("DIME");
            x.SetDisplayName("DIME");
            x.SetDescription("Data in Motion Enterprise");
            x.OnException(ex => HostLogger.Get<TransporterService>().Fatal(ex));
            x.UnhandledExceptionPolicy = UnhandledExceptionPolicyCode.LogErrorAndStopService;
            x.UseNLog();
        });
    }
}