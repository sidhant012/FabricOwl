using FabricOwl.IConfigs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FabricOwl
{
    public class RCAEngine
    {
        //Use this class to create the RCAEngine
        public List<ConcurrentEvents> GetSimultaneousEventsForEvent
            (IEnumerable<ConcurrentEventsConfig> configs, List<CombinedSFItems> inputEvents, List<CombinedSFItems> events, List<ConcurrentEvents> existingEvents = null)
        {
            /*
                Grab the events that occur concurrently with an inputted current event.
            */
            List<ConcurrentEvents> simulEvents = new List<ConcurrentEvents>();

            if (existingEvents != null)
            {

                simulEvents.AddRange(existingEvents);
            }

            // iterate through all the input events
            foreach (var inputEvent in inputEvents)
            {
                if(FindEvent(simulEvents, inputEvent) != null)
                {
                    continue;
                }

                var action = "";
                var reasonForEvent = "";
                ConcurrentEvents reason = null;
                var moreSpecificReason = "";

                foreach (ConcurrentEventsConfig config in configs)
                {
                    string parsed = "";
                    if (config.EventType == inputEvent.Kind.ToString())
                    {
                        // iterate through all events to find relevant ones
                        if(GetPropertyValues(inputEvent, config.Result) != null)
                        {
                            parsed = GetPropertyValues(inputEvent, config.Result);
                            if(config.ResultTransform != null)
                            {
                                parsed = Transformations.getTransformations(config.ResultTransform, parsed);
                            }
                            action = parsed;
                        }

                        reasonForEvent = action;
                        foreach(RelevantEventsConfig relevantEventType in config.RelevantEventsType)
                        {
                            if(relevantEventType.EventType == "self")
                            { // self referential events logic starts here
                                bool propMaps = true;
                                var mappings = relevantEventType.PropertyMappings;
                                foreach(var mapping in mappings)
                                {
                                    object sourceVal = GetPropertyValues(inputEvent, (string)mapping.SourceProperty);
                                    object targetVal = mapping.TargetProperty;

                                    if(mapping.SourceTransform != null)
                                    {
                                        sourceVal = Transformations.getTransformations(mapping.SourceTransform, (string)sourceVal);
                                    }

                                    if(sourceVal == null || targetVal == null || sourceVal != targetVal)
                                    {
                                        propMaps = false;
                                    }
                                }

                                if(propMaps)
                                {
                                    if(relevantEventType.SelfTransform != null)
                                    {
                                        parsed = Transformations.getTransformations(relevantEventType.SelfTransform, parsed);
                                    }

                                    if(reason == null)
                                    {
                                        reason.Name = "self";
                                        reason.RelatedEvent = null;
                                    }
                                    action = parsed;
                                    if(!string.IsNullOrEmpty(relevantEventType.Result))
                                    {
                                        moreSpecificReason = relevantEventType.Result;
                                    }

                                    reasonForEvent = action;
                                }
                            } //self referential events logic ends here
                            foreach(var iterEvent in events) //iterate through other events to find relationships
                            {
                                if (relevantEventType.EventType == iterEvent.Kind.ToString())
                                {
                                    // see if each property mapping holds true
                                    bool valid = true;
                                    var mappings = relevantEventType.PropertyMappings;
                                    foreach (var mapping in mappings)
                                    {
                                        var sourceVal = GetPropertyValues(inputEvent, (string)mapping.SourceProperty);
                                        if (mapping.SourceTransform != null)
                                        {
                                            sourceVal = Transformations.getTransformations(mapping.SourceTransform, (string)sourceVal);
                                        }

                                        var targetVal = GetPropertyValues(iterEvent, (string)mapping.TargetProperty);
                                        if (mapping.TargetTransform != null)
                                        {
                                            targetVal = Transformations.getTransformations(mapping.TargetTransform, (string)targetVal);
                                        }

                                        if (sourceVal == null || targetVal == null || sourceVal != targetVal)
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
                                            var reasons = GetSimultaneousEventsForEvent(configs, new List<CombinedSFItems>{iterEvent }, events, simulEvents);
                                            foreach (ConcurrentEvents r in reasons)
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
                ConcurrentEvents tempEvent = new ConcurrentEvents() { Kind = inputEvent.Kind, Name = inputEvent.Name, RelatedEvent = reason, 
                    ReasonForEvent = (string.IsNullOrEmpty(moreSpecificReason) ? reasonForEvent : moreSpecificReason), EventInstanceId = inputEvent.EventInstanceId, 
                    TimeStamp = inputEvent.TimeStamp, InputEvent = inputEvent};
                simulEvents.Add(tempEvent);
            }
            return simulEvents;
        }

        private ConcurrentEvents FindEvent(List<ConcurrentEvents> events, CombinedSFItems tempEvent) {
            return events.FirstOrDefault(e => e.EventInstanceId == tempEvent.EventInstanceId.ToString());
        }

        private ConcurrentEvents FindEventReasons(List<ConcurrentEvents> events, ConcurrentEvents reason)
        {
            return events.FirstOrDefault(e => e.EventInstanceId == reason.EventInstanceId.ToString());
        }

        //Rewrite the Utils.result method
        public dynamic GetPropertyValues(CombinedSFItems src, string propName)
        {
            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValues(GetPropertyValues(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
    }
}
