﻿using System;
using System.Threading;
using Caliburn.Micro;
using Haris.Core.Events.Command;
using Haris.Core.Events.Intent;
using Haris.Core.Events.MySensors;
using Haris.Core.Services.Logging;

namespace Haris.Core.Modules.ConsoleInput
{
    [DisableModule]
	public class ConsoleCommandInputModule: HarisModuleBase<IntentRecognitionCompletionEvent>
	{
		private readonly IEventAggregator _eventAggregator;
		private readonly CancellationTokenSource _cts;

		public ConsoleCommandInputModule(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
			_cts = new CancellationTokenSource();
		}

		public override void Dispose()
		{
			_cts.Cancel();
		}

		public override void Init()
		{
			RunInBusyContextWithErrorFeedback(() =>
			{
				while (_cts.IsCancellationRequested == false)
				{
					Logger.LogPrompt("Type commands to send to LUIS:");
					var cmd = Console.ReadLine();
					if (string.IsNullOrWhiteSpace(cmd) == false)
					{
						//_eventAggregator.Publish(new CommandTextAcquiredEvent(cmd));
						_eventAggregator.Publish(new AttributedMessageEvent(cmd));
					}
					else if(cmd == null)
					{
						break;
					}
				}
			}, _cts.Token);
			_eventAggregator.Subscribe(this);
		}

		public override void Handle(IntentRecognitionCompletionEvent message)
		{
			RunInBusyContextWithErrorFeedback(() =>
			{
				var result = message.Payload;
				Logger.LogInfo(string.Format("{0} th:{1} r:{4} pr:{2} n:{3}", result.IntentLabel, result.ThingParameter,
					result.PropertyParameter, result.NumericParameter, result.RoomParameter));
			}, _cts.Token);
		}
	}
}