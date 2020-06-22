using SWNetwork;
using UnityEngine;

namespace Llamo.Health
{
    public class Health : LocalHealth
    {
        #region Network
        private NetworkID networkID;
        private SyncPropertyAgent syncPropertyAgent;
        private RemoteEventAgent remoteEventAgent;

        private const string HEALTH_CHANGED = "health_changed";
        private const string KILLED_EVENT = "killed";
        #endregion

        protected override void Awake()
        {
            networkID = GetComponent<NetworkID>();
            syncPropertyAgent = GetComponent<SyncPropertyAgent>();
            remoteEventAgent = GetComponent<RemoteEventAgent>();

            base.Awake();
        }

        public void RemoteHealthChanged()
        {
            // Update the hpSlider when player hp changes
            int newHealth = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).GetIntValue();

            bool wasPlayerDamaged = newHealth < currentHealth;
            currentHealth = newHealth;
            CalculatePercentage();

            // we only instantiate blood when it's damaged, not healing
            if (wasPlayerDamaged)
            {
                // damaged performed
                AddDamageSFX(cameraParticleTransform.position);
            }

            if (networkID.IsMine && wasPlayerDamaged)
            {
                cameraShake.actionShakeCamera();

                if (newHealth <= 0)
                {
                    // invoke the "killed" remote event when hp is 0. 
                    remoteEventAgent.Invoke(KILLED_EVENT);
                }
            }
        }

        public void OnHpReady()
        {
            currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).GetIntValue();
            int version = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).version;

            // If version is 0, you can call the Modify() method on the SyncPropertyAgent to initialize player's hp to maxHp.
            if (version == 0)
            {
                syncPropertyAgent.Modify(HEALTH_CHANGED, maxHealth);
                currentHealth = maxHealth;
            }

            CalculatePercentage();
        }

        public void OnDamageConflict(SWSyncConflict conflict, SWSyncedProperty property)
        {
            // 1
            int newLocalHP = (int)conflict.newLocalValue;
            int oldLocalHP = (int)conflict.oldLocalValue;
            int remoteHP = (int)conflict.remoteValue;

            // 2
            // check if player is already killed
            if (remoteHP == 0)
            {
                property.Resolve(0);
                return;
            }

            // 3
            // should use remoteHP instead of oldLocalHP to apply damage
            int damage = oldLocalHP - newLocalHP;
            int resolvedHP = remoteHP - damage;
            if (resolvedHP < 0)
            {
                resolvedHP = 0;
            }
            property.Resolve(resolvedHP);
        }

        public void RemoteThrowGun(SWNetworkMessage msg)
        {
            Gun gunTarget = transform.GetComponentInChildren<Gun>();
            if (gunTarget != null)
            {
                Debug.Log("remote throwing gun: ");
                Vector3 direction = msg.PopVector3();
                gunTarget.Throw(400f, direction);
            }
        }

        protected override void CreateRagdoll()
        {
            NetworkClient.Instance.LastSpawner.SpawnForNonPlayer((int)NonPlayerIndexes.Player_Corpse, transform.position, Quaternion.identity);
        }

        #region IHealth

        public override void GiveHealth(int amount, Vector3 impactPosition)
        {
            int newHealth = currentHealth + amount;

            if (newHealth > maxHealth)
            {
                newHealth = maxHealth;
            }

            AddHealSFX(cameraParticleTransform.position);

            // Apply damage and modify the "heal" SyncProperty.
            syncPropertyAgent?.Modify(HEALTH_CHANGED, newHealth);
        }

        public override void PerformDamage(int damage, Vector3 impactPosition)
        {
            if (isPlayerInmune)
            {
                return;
            }

            //currentHealth = syncPropertyAgent.GetPropertyWithName(HEALTH_CHANGED).GetIntValue();
            int newHealth = currentHealth - damage;

            // if hp is lower than 0, set it to 0.
            if (newHealth < 0)
            {
                newHealth = 0;
            }

            // Apply damage and modify the "damage" SyncProperty.
            syncPropertyAgent?.Modify(HEALTH_CHANGED, newHealth);
        }

        #endregion
    }
}