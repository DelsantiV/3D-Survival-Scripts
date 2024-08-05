using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarryable
{
    void SpawnInWorld(Vector3 spawnPosition);

    void Action(PlayerManager player);
}

