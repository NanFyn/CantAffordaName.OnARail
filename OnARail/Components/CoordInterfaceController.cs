using UnityEngine;

namespace OnARail.Components
{
    public class CoordInterfaceController : MonoBehaviour
    {
        private NomaiCoordinateInterface _interface;

        //Coords start at top left, increments clockwise
        private int[] coord1 = new int[] { 0, 1, 2, 3 };
        private int[] coord2 = new int[] { 5, 4, 3, 2 };
        private int[] coord3 = new int[] { 0, 1, 2, 3, 4, 5 };

        void Awake()
        {
            GameObject.Destroy(GetComponentInChildren<EyeCoordinatePromptTrigger>().gameObject);
            _interface = GetComponent<NomaiCoordinateInterface>();

            _interface._raisePillarSlot.OnSlotActivated += (NomaiInterfaceSlot _) =>
            {
                foreach (var node in _interface._nodeControllers)
                {
                    //Eye coords cause a soft lock, have to reset
                    node.ResetNodes();
                }
                _interface.SetPillarRaised(true, true);
            };
            _interface._lowerPillarSlot.OnSlotActivated += (NomaiInterfaceSlot _) => _interface.SetPillarRaised(false, false);
            _interface._lowerPillarSlot.OnSlotActivated += (NomaiInterfaceSlot _) => CheckIfCorrect();
        }

        void Start()
        {
            _interface._upperOrb.SetOrbPosition(_interface._raisePillarSlot.transform.TransformPoint(new Vector3(0, 2.6f, 0)));
        }

        void CheckIfCorrect()
        {
            OnARail.DebugLog("CheckIfCorrect", OWML.Common.MessageType.Info);
            if (IsCorrect())
            {
                OnARail.DebugLog("Correctly entered the coords!", OWML.Common.MessageType.Success);
                //Doesn't deactivate properly, destroy it
                Destroy(_interface._upperOrb);
                OnARail.SolvedCoords();
            }
            else {OnARail.DebugLog("The coords are not right.", OWML.Common.MessageType.Success);}
        }

        bool IsCorrect()
        {
            bool correct = CheckCoords();
            return correct;
        }

        bool CheckCoords()
        {
            bool first = _interface._nodeControllers[0].CheckCoordinate(coord1);
            bool second = _interface._nodeControllers[1].CheckCoordinate(coord2);
            bool third = _interface._nodeControllers[2].CheckCoordinate(coord3);
            OnARail.DebugLog("1: " + first + " | 2: " + second + " | 3: " + third, OWML.Common.MessageType.Info);

            return first && second && third;
        }
    }
}