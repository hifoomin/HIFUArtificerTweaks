using HAT;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFUArtificerTweaks.Projectiles
{
    public static class WallOfInfernoProjectile
    {
        public static GameObject prefab;
        public static float damage = Main.flamewallDamage.Value;
        public static float procCoeff = Main.flamewallProcCoeff.Value;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElementalRings/FireTornado.prefab").WaitForCompletion(), "WallOfInfernoPillar");
            prefab.transform.eulerAngles = new Vector3(0, 0, 90);

            Object.Destroy(prefab.GetComponent<SphereCollider>());

            var cc = prefab.AddComponent<CapsuleCollider>();
            cc.isTrigger = false;
            cc.center = new Vector3(0f, 0f, 0f);
            cc.radius = 1f;
            cc.height = 1f;

            // add collider for gravity

            var hitbox = prefab.transform.GetChild(0);
            hitbox.transform.localScale = new Vector3(8.5f, 8.5f, 20f);
            hitbox.transform.localPosition = new Vector3(0, 0f, 8f);

            // add hitbox

            var rb = prefab.GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.useGravity = true;
            rb.freezeRotation = true;

            // add rb for gravity

            var cf = prefab.AddComponent<ConstantForce>();
            cf.force = new Vector3(0f, -2500f, 0f);

            // add gravity real

            var psoi = prefab.AddComponent<ProjectileStickOnImpact>();
            psoi.ignoreCharacters = true;
            psoi.ignoreWorld = false;
            psoi.alignNormals = false;

            var ps = prefab.GetComponent<ProjectileSimple>();
            ps.lifetime = 7f;

            ProjectileDamage pd = prefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.IgniteOnHit;

            ProjectileOverlapAttack overlap = prefab.GetComponent<ProjectileOverlapAttack>();
            overlap.damageCoefficient = damage;
            overlap.resetInterval = 1f;
            overlap.overlapProcCoefficient = procCoeff;

            ProjectileController projectileController = prefab.GetComponent<ProjectileController>();
            GameObject ghostPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MageFirePillarGhost.prefab").WaitForCompletion(), "WallOfInfernoPillarGhost", false);
            // Main.HATLogger.LogError("ghost prefab is " + ghostPrefab);

            var pillar = ghostPrefab.transform.GetChild(0);
            pillar.localScale = new Vector3(2.5f, 4f, 2.5f);

            var pillarParticleSystem = pillar.GetComponent<ParticleSystem>();

            var pillarVelocity = pillarParticleSystem.velocityOverLifetime;
            pillarVelocity.speedModifier = 0.5f;

            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0f, 0.0f), new GradientAlphaKey(1f, 0.7f), new GradientAlphaKey(1f, 0.9f), new GradientAlphaKey(0f, 1.0f) });

            var pillarColor = pillarParticleSystem.colorOverLifetime;
            pillarColor.color = gradient;

            ghostPrefab.transform.eulerAngles = new Vector3(0, 0, 90);
            var destroyOnTimer = ghostPrefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 7f;

            projectileController.ghostPrefab = ghostPrefab;

            ContentAddition.AddProjectile(prefab);
        }
    }
}