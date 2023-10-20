using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioBlock_Example2 : BioBlock
{
    public override void TakeStep()
    {
        base.TakeStep();

        if (CheckBioBlockFlag())
        {
            SpawnNewBioBlockWithFlag(GetStoredDirection());
            if (birthstep % 5 == 0)
                ActivateTrait();
        }
        else if (birthstep % 5 == 0)
        {
            SetBioBlockFlag();
            if (birthstep % 2 == 0)
                StoreDirection(Direction.UpRight);
            else
                StoreDirection(Direction.UpLeft);

            SpawnNewBioBlock(Direction.Forward);
        }
        else
        {
            SpawnNewBioBlock(Direction.Forward);
        }

    }
}
