using System;
using System.Collections.Generic;
using Haris.Core.Events.AttachedProperties;

namespace Haris.Core.Events
{
	public interface IEvent
	{
		Guid Id { get; }
		IEnumerable<IAttachedProperty> AttachedProperties { get; }
	}

	public interface IEvent<out TPayload>
	{
		TPayload Payload { get; }
	}
}