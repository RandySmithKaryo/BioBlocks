using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioBlock_Example4 : BioBlock
{
    public override void TakeStep()
    {
        base.TakeStep();

        if (generation % 4 == 0)
        {
            ActivateTrait();
        }
        else
        {
            DeactivateTrait();
        }

        if (NeighborCountAbove(5))
        {
            RemoveThisBioBlock();
        }
        else if ((birthstep < 10) && (age < 5))
        {
            SpawnNewBioBlock(Direction.Forward);
            SpawnNewBioBlock(Direction.Back);
            SpawnNewBioBlock(Direction.Left);
            SpawnNewBioBlock(Direction.Right);
            SpawnNewBioBlock(Direction.Up);
            SpawnNewBioBlock(Direction.Down);
        }
        else if (NeighborCountWithinRange(3,6))
        {
            SpawnNewBioBlock(Direction.Forward);
            SpawnNewBioBlock(Direction.Back);
            SpawnNewBioBlock(Direction.Left);
            SpawnNewBioBlock(Direction.Right);
            SpawnNewBioBlock(Direction.Up);
            SpawnNewBioBlock(Direction.Down);
        }
        else if (NeighborCountBelow(3))  
        {
            if (birthstep % 2 == 0)
                SpawnNewBioBlock(GetDirectionTowardsOrigin());
            else
                SpawnNewBioBlock(GetDirectionAwayFromOrigin());
        }
    }
}
