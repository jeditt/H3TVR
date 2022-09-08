using BepInEx;
using UnityEngine;
using Valve.VR;
using System.Collections;
using FistVR;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace H3TVR
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
	[BepInProcess("h3vr.exe")]
    public class H3TVR : BaseUnityPlugin
	{
        private const float SlowdownFactor = .001f;
        private const float SlowdownLength = 6f;
        public string SlomoStatus = "Off";
        private const float MaxSlomo = .1f;
        private const float SlomoWaitTime = 2f;
        private const float ZeroGWaitTime = 3f;
        private const float RealisticFallTime = 1f;
        private string ZeroGStatus = "Off";
        private readonly Hooks _hooks;
        private List<string> GunList = File.ReadAllLines(@"c:\H3TVR\GunList.txt").ToList();
        private List<string> MagazineList = File.ReadAllLines(@"c:\H3TVR\MagazineList.txt").ToList();

        public H3TVR()
		{
			_hooks = new Hooks();
			_hooks.Hook();
            Logger.LogInfo("Loading H3TVR");
        }

		private void Awake()
		{      
            Logger.LogInfo("Successfully loaded H3TVR!");  
        }

		private void Update()
		{
            //wonderful toy spawn
            if (Input.GetKeyDown(KeyCode.H))

            {
                SpawnWonderfulToy();
            }

            //body pillow spawn
            if (Input.GetKeyDown(KeyCode.J))

            {
                SpawnPillow();
            }

            //flash spawn
            if (Input.GetKeyDown(KeyCode.K))
            {
                SpawnFlash();
            }

            //shuri spawn
            if (Input.GetKey(KeyCode.B))
            {
                SpawnShuri();
            }

            //nade spawn
            if (Input.GetKeyDown(KeyCode.V))
            {
                SpawnNadeRain();
            }

            //hydration spawn
            if (Input.GetKeyDown(KeyCode.I))
            {
                SpawnHydration();
            }

            //jedit tt spawn
            if (Input.GetKeyDown(KeyCode.U))
            {
                SpawnJeditToy();
            }

            if (GM.CurrentMovementManager.Hands[1].Input.AXButtonDown || Input.GetKeyDown(KeyCode.Space))
            {
                //Logger.LogInfo("Detected Right A Press!");
                SlomoStatus = "Slowing";
            }

            if (SlomoStatus == "Slowing")
            {
                //Logger.LogInfo("Slowing!");
                SlomoScaleDown();
            }

            if (SlomoStatus == "Wait")
            {
                //Logger.LogInfo("Waiting!");
                SlomoStatus = "Paused";
                StartCoroutine(SlomoWait());
            }

            if (SlomoStatus == "Return")
            {
                //Logger.LogInfo("Returning!");
                SlomoReturn();
            }

            if (Time.timeScale == 1)
            {
                SlomoStatus = ("Off");
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                DestroyHeld();
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                SpawnSkittySubGun();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                ZeroGravityBumpDown();
            }

            if (ZeroGStatus == "On")
            {
                StartCoroutine(ZeroGWait());
            }

            if (ZeroGStatus == "Falling")
            {
                StartCoroutine(RealisticFallWait());
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                EnableMeatHands();
            }



        }

        private void SpawnWonderfulToy()
        {
            // Get the object you want to spawn
            FVRObject obj = IM.OD["TippyToyAnton"];


            // Instantiate (spawn) the object above the player's right hand
            GameObject go = Instantiate(obj.GetGameObject(), new Vector3(0f, .25f, 0f) + GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);

            //add some speeeeen
            go.GetComponent<Rigidbody>().AddTorque(new Vector3(.25f, .25f, .25f));


            //add force
            go.GetComponent<Rigidbody>().AddForce(GM.CurrentPlayerBody.Head.forward * 25);
        }

        private void SpawnJeditToy()
        {
            // Get the object you want to spawn
            FVRObject obj = IM.OD["JediTippyToy"];


            // Instantiate (spawn) the object above the player's right hand
            GameObject go = Instantiate(obj.GetGameObject(), new Vector3(0f, .25f, 0f) + GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);

            //add some speeeeen
            go.GetComponent<Rigidbody>().AddTorque(new Vector3(.25f, .25f, .25f));


            //add force
            go.GetComponent<Rigidbody>().AddForce(GM.CurrentPlayerBody.Head.forward * 25);
        }

        private void SpawnPillow()
        {
            // Get the object you want to spawn
            FVRObject obj = IM.OD["BodyPillow"];


            // Instantiate (spawn) the object above the player head
            GameObject go = Instantiate(obj.GetGameObject(), new Vector3(0f, .25f, 0f) + GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);


            //add force
            go.GetComponent<Rigidbody>().AddForce(GM.CurrentPlayerBody.Head.forward * 4000);
        }

        //we want to spawn a flashbang infront of the player with little notice
        private void SpawnFlash()
        {
            // Get the object you want to spawn
            FVRObject obj = IM.OD["PinnedGrenadeXM84"];


            // Instantiate (spawn) the object above the player head
            Logger.LogInfo("Spawned Object");
            GameObject go = Instantiate(obj.GetGameObject(), new Vector3(0f, .25f, 0f) + GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);


            //prime the flash object
            Logger.LogInfo("Getting Component");
            PinnedGrenade grenade = go.GetComponentInChildren<PinnedGrenade>();
            Logger.LogInfo("Releasing Lever");
            grenade.ReleaseLever();



            //add force
            Logger.LogInfo("Adding Force");
            go.GetComponent<Rigidbody>().AddForce(GM.CurrentPlayerBody.Head.forward * 500);
        }

        private void SpawnNadeRain()
        {
            //    //Set cartridge speed
            float howFast = 15.0f;

            //    //Set max angle
            float maxAngle = 4.0f;

            Transform PointingTransfrom = transform;

            //    //Get Random direction for bullet
            Vector2 randRot = Random.insideUnitCircle;

            // Random number for pull chance
            int pullChance = Random.Range(1, 11);
            Logger.LogInfo(pullChance);

            // Get the object you want to spawn
            FVRObject obj = IM.OD["PinnedGrenadeM67"];

            //Set Object Position
            Vector3 grenadePosition0 = GM.CurrentPlayerBody.Head.position + (GM.CurrentPlayerBody.Head.up * 0.02f);

            // Instantiate (spawn) the object above the player head
            Logger.LogInfo("Spawned Object");
            GameObject go = Instantiate(obj.GetGameObject(), grenadePosition0, Quaternion.LookRotation(GM.CurrentPlayerBody.Head.up));

            //Set Object Direction
            go.transform.Rotate(new Vector3(randRot.x * maxAngle, randRot.y * maxAngle, 0.0f), Space.Self);

            //add force
            Logger.LogInfo("Adding Force");
            go.GetComponent<Rigidbody>().velocity = go.transform.forward * howFast;


            if (pullChance == 10)
            {
                //prime the grenade object
                Logger.LogInfo("Getting Component");
                PinnedGrenade grenade = go.GetComponentInChildren<PinnedGrenade>();
                Logger.LogInfo("Releasing Lever");
                grenade.ReleaseLever();
            }

        }

        private void SpawnShuri()

        {
            //Set cartridge speed
            float howFast = 30.0f;

            //Set max angle
            float maxAngle = 4.0f;

            Transform PointingTransfrom = transform;



            //Get Random direction for bullet
            Vector2 randRot = Random.insideUnitCircle;

            // Get the object I want to spawnz
            FVRObject obj = IM.OD["Shuriken"];

            //Set Object Position
            Vector3 shuriPosition0 = GM.CurrentPlayerBody.Head.position + (GM.CurrentPlayerBody.Head.forward * 0.02f);


            //Create Bullet
            //GameObject go0 = Instantiate(obj.GetGameObject(), bulletPosition0, Quaternion.LookRotation(-GM.CurrentPlayerBody.LeftHand.upxx));
            //GameObject go1 = Instantiate(obj.GetGameObject(), bulletPosition0, Quaternion.LookRotation(-GM.CurrentPlayerBody.LeftHand.up));

            //old spray
            GameObject go0 = Instantiate(obj.GetGameObject(), shuriPosition0, Quaternion.LookRotation(GM.CurrentPlayerBody.Head.forward));


            //Set Object Direction
            go0.transform.Rotate(new Vector3(randRot.x * maxAngle, randRot.y * maxAngle, 0.0f), Space.Self);


            //Add Force


            //go0.GetComponent<Rigidbody>().velocity = GM.CurrentPlayerBody.LeftHand.forward * howFast;
            //go1.GetComponent<Rigidbody>().velocity = GM.CurrentPlayerBody.LeftHand.forward * howFast;

            //old spray
            go0.GetComponent<Rigidbody>().velocity = go0.transform.forward * howFast;


        }

        private void SlomoScaleDown()
        {
            if (Time.timeScale > MaxSlomo)
            {
                Time.timeScale -= (1f) * Time.unscaledDeltaTime;
                Time.fixedDeltaTime = Time.timeScale / SteamVR.instance.hmd_DisplayFrequency;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            }

            if (Time.timeScale <= MaxSlomo)
            {
                SlomoStatus = ("Wait");
            }
        }

        private void SlomoReturn()
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale += (1f / 3f) * Time.unscaledDeltaTime;
                Time.fixedDeltaTime = Time.timeScale / SteamVR.instance.hmd_DisplayFrequency;
                Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
            }
        }

        IEnumerator SlomoWait()
        {
            yield return new WaitForSecondsRealtime(SlomoWaitTime);
            SlomoStatus = "Return";
        }

        IEnumerator ZeroGWait()
        {
            yield return new WaitForSecondsRealtime(ZeroGWaitTime);
            ZeroGStatus = "Falling";
            RealisticFall();
        }

        IEnumerator RealisticFallWait()
        {
            yield return new WaitForSecondsRealtime(RealisticFallTime);
            ZeroGravityBumpUp();
        }

        //private void SpawnNade()
        //{

        //}

        private void SpawnHydration()
        {
            // Get the object you want to spawn
            FVRObject obj = IM.OD["SuppressorBottle"];


            // Instantiate (spawn) the object above the player's right hand
            GameObject go = Instantiate(obj.GetGameObject(), new Vector3(0f, .25f, 0f) + GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);

            //add some speeeeen
            go.GetComponent<Rigidbody>().AddTorque(new Vector3(.25f, .25f, .25f));


            //add force
            go.GetComponent<Rigidbody>().AddForce(GM.CurrentPlayerBody.Head.forward * 25);
        }

        private void DestroyHeld()

        {
            if (GM.CurrentMovementManager.Hands[1].CurrentInteractable != null && GM.CurrentMovementManager.Hands[1].CurrentInteractable is FVRPhysicalObject)
            {
                Destroy(GM.CurrentMovementManager.Hands[1].CurrentInteractable.gameObject);
            }

            //Set max angle
            float maxAngle = 4.0f;

            Transform PointingTransfrom = transform;



            //Get Random direction for bullet
            Vector2 randRot = Random.insideUnitCircle;

            // Get the object I want to spawnz
            FVRObject obj = IM.OD["12GaugeShellFreedomfetti"];

            //Set Object Position
            Vector3 shellPosition0 = GM.CurrentPlayerBody.RightHand.position + (GM.CurrentPlayerBody.RightHand.forward * 0.02f);

            GameObject go0 = Instantiate(obj.GetGameObject(), shellPosition0, Quaternion.LookRotation(GM.CurrentPlayerBody.RightHand.forward));


            //Set Object Direction
            go0.transform.Rotate(new Vector3(randRot.x * maxAngle, randRot.y * maxAngle, 0.0f), Space.Self);

            //Detonate Shell?
            FVRFireArmRound cartridge = go0.GetComponent<FVRFireArmRound>();
            cartridge.Splode(0.01f, false, true);

        }

        private void SpawnSkittySubGun()
        {
            GunList.Shuffle();
            MagazineList.Shuffle();
            string TopGun = GunList.ElementAt(0);
            string TopGunTruncated = new string(TopGun.Take(5).ToArray());
            Logger.LogInfo(TopGunTruncated);
            Logger.LogInfo(TopGun);
            string MatchingMagazine = MagazineList.Find(o => o.Contains(TopGunTruncated));
            Logger.LogInfo(MatchingMagazine);

            // Get the object you want to spawn
            FVRObject obj = IM.OD[TopGun];
            FVRObject obj2 = IM.OD[MatchingMagazine];

            // Instantiate (spawn) the object above the player's head
            GameObject go = Instantiate(obj.GetGameObject(), new Vector3(0f, .25f, 0f) + GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);
            GameObject go2 = Instantiate(obj2.GetGameObject(), new Vector3(0f, .25f, 0f) + GM.CurrentPlayerBody.Head.position, GM.CurrentPlayerBody.Head.rotation);

            //add some speeeeen
            go.GetComponent<Rigidbody>().AddTorque(new Vector3(.25f, .25f, .25f));
            go2.GetComponent<Rigidbody>().AddTorque(new Vector3(.25f, .25f, .25f));

            //add force
            go.GetComponent<Rigidbody>().AddForce(GM.CurrentPlayerBody.Head.forward * 100);
            go2.GetComponent<Rigidbody>().AddForce(GM.CurrentPlayerBody.Head.forward * 100);

        }

        private void ZeroGravityBumpDown()
        {
            GM.Options.SimulationOptions.PlayerGravityMode = SimulationOptions.GravityMode.None;
            GM.Options.SimulationOptions.ObjectGravityMode = SimulationOptions.GravityMode.None;
            GM.CurrentSceneSettings.RefreshGravity();           
            StartCoroutine(ZeroGWait());
            //Logger.LogInfo("Gravity Is Now " + GM.Options.SimulationOptions.PlayerGravityMode);

        }

        private void ZeroGravityBumpUp()
        {
            GM.Options.SimulationOptions.PlayerGravityMode = SimulationOptions.GravityMode.Playful;
            GM.Options.SimulationOptions.ObjectGravityMode = SimulationOptions.GravityMode.Playful;
            GM.CurrentSceneSettings.RefreshGravity();
            ZeroGStatus = "Off";
            //Logger.LogInfo("Gravity Is Now " + GM.Options.SimulationOptions.PlayerGravityMode);
        }

        private void RealisticFall()
        {
            GM.Options.SimulationOptions.PlayerGravityMode = SimulationOptions.GravityMode.Realistic;
            GM.Options.SimulationOptions.ObjectGravityMode = SimulationOptions.GravityMode.Realistic;
            GM.CurrentSceneSettings.RefreshGravity();
            //Logger.LogInfo("Gravity Is Now " + GM.Options.SimulationOptions.PlayerGravityMode);
        }

        private void EnableMeatHands()
        {
            GM.Options.ControlOptions.MFMode = ControlOptions.MeatFingerMode.Enabled;
        }

        private void OnDestroy()
        {
            _hooks.Unhook();
        }
    }
}




