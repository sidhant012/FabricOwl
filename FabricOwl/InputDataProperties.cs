using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace FabricOwl
{
    public class InputDataProperties
    {

        public dynamic RepairTasksPropertiesTransformations(dynamic RepairTaskData)
        {
            foreach(var repair in RepairTaskData)
            {
                repair.Kind = "RepairTask";
                repair.EventInstanceId = repair.TaskId;
                repair.TimeStamp = repair.History.CreatedUtcTimestamp;
                repair.Name = "";
                repair.TaskId = repair.TaskId;
            }
            return RepairTaskData;
        }

        public dynamic EventStorePropertiesTransformations(dynamic EventStoreTaskData)
        {
            foreach(var events in EventStoreTaskData)
            {
                events.NodeInstance = (string)events.NodeInstance;
                events.ExitCode = (string)events.ExitCode;
                events.UnexpectedTermination = (string) events.UnexpectedTermination;
            }
            return EventStoreTaskData;
        }

        public dynamic CombineData(List<dynamic> inputData) 
        {
            dynamic mergeData = inputData.ElementAt(0);
            foreach (var inputItem in inputData.Skip(1))
            {
                foreach (var item in inputItem)
                {
                    mergeData.Add(item);
                }
            }
            return mergeData;
        }
    }
}
