using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tritium.Concepts
{
    public interface IEntityTask
    {
        public bool IsPossible();

        public string GetName(Entity target);

        public void DoTask(Entity target);
    }

    public class DoorEntity : Entity, IEntityTaskGiver
    {
        public bool isOpen = false;

        public List<IEntityTask> GetTasks()
        {
            return new List<IEntityTask>() { new DoorStateTask() };
        }
    }

    public class DoorStateTask : IEntityTask
    {
        public void DoTask(Entity target)
        {
            if (target is DoorEntity door)
                door.isOpen = !door.isOpen;
        }

        public string GetName(Entity target)
        {
            if (target is DoorEntity door)
                return $"{(door.isOpen ? "Close" : "Open")} Door";

            return null;
        }

        public bool IsPossible() => true;
    }

    public interface IEntityTaskGiver
    {
        public List<IEntityTask> GetTasks();
    }
}
