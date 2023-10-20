using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioBlock_Example3 : BioBlock
{
    public override void TakeStep()
    {
        base.TakeStep();

        if (age > 14)
        {
            if (age % 3 == 0)
                ActivateTrait();
            else
                DeactivateTrait();
        }
        else if (age > 12) 
        {
            if ((GetDistanceFromOrigin() < 4f) || ((GetDistanceFromOrigin() > 5f)))
                RemoveThisBioBlock();
        }
        else if (birthstep < 10)
        {
            SpawnNewBioBlock(Direction.Forward);
            SpawnNewBioBlock(Direction.ForwardRight);
            SpawnNewBioBlock(Direction.Right);
            SpawnNewBioBlock(Direction.BackRight);
            SpawnNewBioBlock(Direction.Back);
            SpawnNewBioBlock(Direction.BackLeft);
            SpawnNewBioBlock(Direction.Left);
            SpawnNewBioBlock(Direction.ForwardLeft);
            
        }
        else
        {
            SpawnNewBioBlock(GetDirectionAwayFromOrigin());
        }
    }
}
