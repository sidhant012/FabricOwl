using FabricOwl.IConfigs;
using FabricOwl.Rules;
using System.Collections.Generic;
using System.Linq;

namespace FabricOwl
{
    public class RCAEngine
    {
        //Use this class to create the RCAEngine
        public List<RCAEvents> GetSimultaneousEventsForEvent
    (IEnumerable<ConcurrentEventsConfig> configs, List<ICommonSFItems> inputEvents, List<ICommonSFItems> events, List<RCAEvents>? existingEvents = null)
        {
            /*
                Grab the events that occur concurrently with an inputted current event.
            */
            List<RCAEvents> simulEvents = new List<RCAEvents>();

            if (existingEvents != null)
            {

                simulEvents.AddRange(existingEvents);
            }

            // iterate through all the input events
            foreach (var inputEvent in inputEvents)
            {
                if (FindEvent(simulEvents, inputEvent) != null)
                {
                    continue;
                }

                var action = "";
                var reasonForEvent = "";
                RCAEvents? reason = null;
                var moreSpecificReason = "";

                foreach (ConcurrentEventsConfig config in configs)
                {
                    string parsed = "";
                    if (config.EventType == inputEvent.Kind)
                    {
                        // iterate through all events to find relevant ones
                        if (GetPropertyValues(inputEvent, config.Result) != null)
                        {
                            parsed = (string)GetPropertyValues(inputEvent, config.Result);
                            if (config.ResultTransform != null)
                            {
                                parsed = Transformations.getTransformations(config.ResultTransform, parsed);
                            }
                            action = parsed;
                        }

                        reasonForEvent = action;
                        foreach (RelevantEventsConfig relevantEventType in config.RelevantEventsType)
                        {
                            if (relevantEventType.EventType == "self")
                            { // self referential events logic starts here
                                bool propMaps = true;
                                var mappings = relevantEventType.PropertyMappings;
                                foreach (var mapping in mappings)
                                {
                                    object sourceVal = GetPropertyValues(inputEvent, mapping.SourceProperty);
                                    object targetVal = mapping.TargetProperty;

                                    if (mapping.SourceTransform != null)
                                    {
                                        sourceVal = Transformations.getTransformations(mapping.SourceTransform, (string)sourceVal);
                                    }

                                    if (sourceVal == null || targetVal == null || sourceVal != targetVal)
                                    {
                                        propMaps = false;
                                    }
                                }

                                if (propMaps)
                                {
                                    if (relevantEventType.SelfTransform != null)
                                    {
                                        parsed = Transformations.getTransformations(relevantEventType.SelfTransform, parsed);
                                    }

                                    if (reason == null)
                                    {
                                        reason.Name = "self";
                                        reason.RelatedEvent = null;
                                    }
                                    action = parsed;
                                    if (!string.IsNullOrEmpty(relevantEventType.Result))
                                    {
                                        moreSpecificReason = relevantEventType.Result;
                                    }

                                    reasonForEvent = action;
                                }
                            } //self referential events logic ends here
                            foreach (var iterEvent in events) //iterate through other events to find relationships
                            {
                                if (relevantEventType.EventType == iterEvent.Kind)
                                {
                                    // see if each property mapping holds true
                                    bool valid = true;
                                    var mappings = relevantEventType.PropertyMappings;
                                    foreach (var mapping in mappings)
                                    {
                                        var sourceVal = GetPropertyValues(inputEvent, mapping.SourceProperty);
                                        if (mapping.SourceTransform != null)
                                        {
                                            sourceVal = Transformations.getTransformations(mapping.SourceTransform, (string)sourceVal);
                                        }

                                        var targetVal = GetPropertyValues(iterEvent, mapping.TargetProperty);
                                        if (mapping.TargetTransform != null)
                                        {
                                            targetVal = Transformations.getTransformations(mapping.TargetTransform, (string)targetVal);
                                        }

                                        if (sourceVal == null || targetVal == null || !sourceVal.Equals(targetVal))
                                        {
                                            valid = false;
                                        }
                                    } //done mapping values and transformations

                                    if (valid)
                                    {
                                        var existingEvent = FindEvent(simulEvents, iterEvent);
                                        if (existingEvent != null)
                                        {
                                            reason = existingEvent;
                                        }
                                        else
                                        {
                                            //generate events to build chain
                                            var reasons = GetSimultaneousEventsForEvent(configs, new List<ICommonSFItems> { iterEvent }, events, simulEvents);
                                            foreach (RCAEvents r in reasons)
                                            {
                                                if (FindEventReasons(simulEvents, r) == null)
                                                {
                                                    simulEvents.Add(r);
                                                }
                                            }
                                            reason = FindEvent(reasons, iterEvent);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Create the tempEvent with all the input values
                //Look into EventProperties , EventProperties = inputEvent.EventProperties
                RCAEvents tempEvent = new RCAEvents()
                {
                    Kind = inputEvent.Kind,
                    Name = (string)GetPropertyValues(inputEvent, "Name"),
                    RelatedEvent = reason,
                    ReasonForEvent = (string.IsNullOrEmpty(moreSpecificReason) ? reasonForEvent : moreSpecificReason),
                    EventInstanceId = inputEvent.EventInstanceId,
                    TimeStamp = inputEvent.TimeStamp,
                    DataType = inputEvent.DataType,
                    InputEvent = inputEvent
                };
                simulEvents.Add(tempEvent);
            }
            return simulEvents;
        }

        private RCAEvents FindEvent(List<RCAEvents> events, ICommonSFItems tempEvent)
        {
            return events.FirstOrDefault(e => e.EventInstanceId == tempEvent.EventInstanceId);
        }

        private RCAEvents FindEventReasons(List<RCAEvents> events, RCAEvents reason)
        {
            return events.FirstOrDefault(e => e.EventInstanceId == reason.EventInstanceId);
        }

        //Rewrite the Utils.result method
        public object GetPropertyValues(ICommonSFItems src, string propName)
        {

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValues((ICommonSFItems)GetPropertyValues(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
    }
}
