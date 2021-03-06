﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.UITest
{
    partial class UITestCase
    {
        void RunTestImpl(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, TaskCompletionSource<RunSummary> tcs)
        {
            // Run on the UI thread
            using (var h = new Handler(Looper.MainLooper))
            {
                h.Post(() =>
                {
                    try
                    {
                        var result = testCase.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);
                        result.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                                tcs.SetException(t.Exception);

                            tcs.SetResult(t.Result);
                        });
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });
            }
        }
    }
}
