using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrototypeBootstrap : MonoBehaviour
{
    const float Cell = 4f;
    Transform world;
    CharacterController controller;
    Camera cam;
    Text status;
    Vector2 moveInput, lookInput;
    bool draggingMove, draggingLook, hidden;
    Vector2 moveStart, lookStart;
    float yaw, pitch, chaseClock;
    GameObject hunter;
    readonly List<GameObject> vents = new List<GameObject>();

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        world = new GameObject("Pizzeria prototype").transform;
        BuildLighting(); BuildRoom(); BuildVents(); BuildPlayer(); BuildUI();
        status.text = "PROTO · visor 3D táctil listo";
    }

    void BuildLighting()
    {
        RenderSettings.ambientLight = new Color(.24f, .25f, .28f);
        RenderSettings.fog = true; RenderSettings.fogColor = new Color(.035f,.045f,.055f); RenderSettings.fogMode = FogMode.Exponential; RenderSettings.fogDensity = .012f;
        var sun = new GameObject("Luces de techo").AddComponent<Light>();
        sun.type = LightType.Directional; sun.intensity = .42f; sun.color = new Color(.75f,.82f,1f); sun.transform.rotation = Quaternion.Euler(47,-35,0);
    }

    void BuildRoom()
    {
        var floor = Cube("Suelo", new Vector3(0,-.35f,20), new Vector3(24,.7f,84), new Color(.15f,.16f,.17f));
        for (int z=0; z<42; z+=2)
            Cube("Baldosa con desgaste", new Vector3((z%4==0?-6:6),.012f,z*Cell/2f), new Vector3(10,.03f,7.8f), z%4==0?new Color(.23f,.23f,.21f):new Color(.11f,.12f,.13f));
        Cube("Pared izquierda",new Vector3(-12,2.8f,20),new Vector3(.55f,5.6f,84),new Color(.25f,.20f,.17f));
        Cube("Pared derecha",new Vector3(12,2.8f,20),new Vector3(.55f,5.6f,84),new Color(.20f,.25f,.24f));
        Cube("Techo",new Vector3(0,5.7f,20),new Vector3(24,.4f,84),new Color(.075f,.085f,.095f));
        for(int i=0;i<8;i++)
        {
            float z=2+i*9.5f;
            Cube("Pilar",new Vector3(-11.3f,2.65f,z),new Vector3(1,5.3f,1.1f),new Color(.12f,.105f,.10f));
            Cube("Pilar",new Vector3(11.3f,2.65f,z),new Vector3(1,5.3f,1.1f),new Color(.11f,.13f,.13f));
            Cube("Lámpara",new Vector3(0,5.38f,z),new Vector3(2.8f,.15f,.7f),new Color(.63f,.49f,.29f),new Color(.7f,.34f,.12f));
        }
        BuildStage(); BuildPartyProps(); BuildClothCurtains();
        hunter=MakeAnimatronic("ENEMIGO · placeholder",new Vector3(0,0,34),new Color(.19f,.20f,.21f));
    }

    void BuildStage()
    {
        Cube("Tarima elevada",new Vector3(0,.32f,73),new Vector3(17,.64f,9),new Color(.22f,.12f,.09f));
        Cube("Borde dorado de escenario",new Vector3(0,.68f,68.7f),new Vector3(17,.17f,.24f),new Color(.72f,.42f,.13f),new Color(.32f,.10f,.015f));
        for(int i=-1;i<=1;i++)
        {
            var c=MakeAnimatronic("Animatrónico en escenario",new Vector3(i*4f,0,73),new Color(.27f,.22f,.17f));
            c.transform.rotation=Quaternion.Euler(0,180,0);
        }
        Cube("Placa pizzería",new Vector3(0,4.6f,68.3f),new Vector3(10,1.3f,.2f),new Color(.48f,.075f,.06f));
    }

    void BuildPartyProps()
    {
        for(int i=0;i<7;i++)
        {
            float x=-8+(i%4)*5.2f,z=9+(i/4)*17f;
            var table=GameObject.CreatePrimitive(PrimitiveType.Cylinder);table.name="Mesa de fiesta";table.transform.SetParent(world);table.transform.position=new Vector3(x,.85f,z);table.transform.localScale=new Vector3(2.5f,.22f,2.5f);Paint(table,new Color(.43f,.08f,.14f));
            var stem=GameObject.CreatePrimitive(PrimitiveType.Cylinder);stem.transform.SetParent(world);stem.transform.position=new Vector3(x,.4f,z);stem.transform.localScale=new Vector3(.35f,.8f,.35f);Paint(stem,new Color(.3f,.31f,.32f));
        }
    }

    void BuildClothCurtains()
    {
        for(int side=-1;side<=1;side+=2)
        {
            var curtain=Cube("Cortina de tela optimizada",new Vector3(side*6.5f,2.1f,68.6f),new Vector3(4,4.2f,.22f),side<0?new Color(.31f,.035f,.05f):new Color(.39f,.045f,.07f));
            curtain.transform.rotation=Quaternion.Euler(0,0,side*4);
            Cube("Pliegue de cortina",curtain.transform.position+new Vector3(side*.3f,0,-.16f),new Vector3(.18f,4,.12f),new Color(.52f,.1f,.11f));
        }
    }

    void BuildVents()
    {
        for(int i=0;i<6;i++)
        {
            float z=7+i*11.5f,x=(i%2==0?-9f:9f);
            var vent=Cube("Ventilación reutilizable",new Vector3(x,.95f,z),new Vector3(2.2f,1.8f,.22f),new Color(.25f,.32f,.34f));
            for(int s=0;s<5;s++)Cube("Rejilla",new Vector3(x,.33f+s*.3f,z-.15f),new Vector3(1.95f,.075f,.12f),new Color(.06f,.08f,.09f));
            vents.Add(vent);
        }
    }

    GameObject MakeAnimatronic(string label,Vector3 at,Color color)
    {
        var root=new GameObject(label);root.transform.SetParent(world);root.transform.position=at;
        var body=GameObject.CreatePrimitive(PrimitiveType.Capsule);body.name="Cuerpo animatrónico";body.transform.SetParent(root.transform);body.transform.localPosition=new Vector3(0,1.05f,0);body.transform.localScale=new Vector3(.8f,.95f,.55f);Paint(body,color);
        var head=GameObject.CreatePrimitive(PrimitiveType.Sphere);head.transform.SetParent(root.transform);head.transform.localPosition=new Vector3(0,2.35f,0);head.transform.localScale=new Vector3(.85f,.77f,.66f);Paint(head,color*1.15f);
        for(int side=-1;side<=1;side+=2)
        {
            var ear=Cube("Oreja",new Vector3(0,0,0),new Vector3(.32f,.85f,.35f),color*1.1f);ear.transform.SetParent(root.transform);ear.transform.localPosition=new Vector3(side*.31f,3.05f,0);ear.transform.localRotation=Quaternion.Euler(0,0,side*-8);
            var eye=GameObject.CreatePrimitive(PrimitiveType.Sphere);eye.name="Ojo brillante";eye.transform.SetParent(root.transform);eye.transform.localPosition=new Vector3(side*.21f,2.4f,-.3f);eye.transform.localScale=Vector3.one*.13f;Paint(eye,new Color(1,.09f,.045f),new Color(.8f,.015f,.003f));
        }
        return root;
    }

    void BuildPlayer()
    {
        var go=new GameObject("Jugador táctil");go.transform.position=new Vector3(0,.15f,4);controller=go.AddComponent<CharacterController>();controller.height=1.7f;controller.radius=.34f;controller.center=new Vector3(0,.85f,0);
        var c=new GameObject("Cámara en primera persona");c.transform.SetParent(go.transform);c.transform.localPosition=new Vector3(0,1.37f,0);cam=c.AddComponent<Camera>();cam.fieldOfView=74;cam.nearClipPlane=.05f;cam.farClipPlane=100;
        c.AddComponent<AudioListener>();yaw=0;
    }

    void BuildUI()
    {
        var canvasObject=new GameObject("HUD táctil",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));var canvas=canvasObject.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        var scaler=canvasObject.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(960,540);
        var events=new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
        status=Label(canvas.transform,"Estado","PROTOTIPO UNITY · ANDROID",22,new Vector2(.5f,.94f),new Vector2(.5f,.94f),new Vector2(0,60),new Color(.76f,.91f,.83f));
        Button(canvas.transform,"hide","ESCONDERSE",new Vector2(.86f,.17f),new Vector2(180,68),ToggleHide);
        Button(canvas.transform,"light","LINTERNA",new Vector2(.86f,.31f),new Vector2(150,58),ToggleLight);
        Label(canvas.transform,"moveHelp","JOYSTICK",15,new Vector2(.14f,.13f),new Vector2(.14f,.13f),new Vector2(150,44),new Color(.68f,.8f,.74f));
    }

    Text Label(Transform parent,string name,string value,int size,Vector2 anchorMin,Vector2 anchorMax,Vector2 dimensions,Color col)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);var rt=go.GetComponent<RectTransform>();rt.anchorMin=anchorMin;rt.anchorMax=anchorMax;rt.sizeDelta=dimensions;
        var text=go.GetComponent<Text>();text.text=value;text.font=Resources.GetBuiltinResource<Font>("Arial.ttf");text.fontSize=size;text.color=col;text.alignment=TextAnchor.MiddleCenter;return text;
    }
    void Button(Transform parent,string name,string text,Vector2 anchor,Vector2 size,UnityEngine.Events.UnityAction action)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(parent,false);var rt=go.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=anchor;rt.sizeDelta=size;
        var image=go.GetComponent<Image>();image.color=new Color(.04f,.10f,.09f,.9f);go.GetComponent<Button>().onClick.AddListener(action);Label(go.transform,"text",text,20,new Vector2(0,0),new Vector2(1,1),Vector2.zero,Color.white);
    }

    GameObject Cube(string label,Vector3 pos,Vector3 scale,Color color,Color? emission=null)
    {
        var obj=GameObject.CreatePrimitive(PrimitiveType.Cube);obj.name=label;obj.transform.SetParent(world);obj.transform.position=pos;obj.transform.localScale=scale;Paint(obj,color,emission??Color.black);return obj;
    }
    void Paint(GameObject obj,Color color,Color emission=default)
    {
        var shader=Shader.Find("Universal Render Pipeline/Lit");if(shader==null)shader=Shader.Find("Standard");var mat=new Material(shader);mat.color=color;
        if(emission.maxColorComponent>.001f){mat.EnableKeyword("_EMISSION");mat.SetColor("_EmissionColor",emission);}obj.GetComponent<Renderer>().material=mat;
    }

    void ToggleLight(){RenderSettings.ambientIntensity=RenderSettings.ambientIntensity>.8f?.38f:1.15f;}
    void ToggleHide(){hidden=!hidden;controller.enabled=!hidden;status.text=hidden?"OCULTO · pulsa ESCONDERSE para salir":"Pizzería · busca los animatrónicos";}

    void Update()
    {
        ReadTouch();
        if(!hidden)
        {
            Vector3 direction=new Vector3(moveInput.x,0,moveInput.y);direction=Quaternion.Euler(0,yaw,0)*direction;
            controller.SimpleMove(direction*(Input.GetKey(KeyCode.LeftShift)?5.0f:3.1f));
            if(hunter!=null&&Time.time>30){chaseClock+=Time.deltaTime;Vector3 to=controller.transform.position-hunter.transform.position;to.y=0;if(to.sqrMagnitude>1.4f){hunter.transform.position+=to.normalized*Time.deltaTime*1.6f;hunter.transform.rotation=Quaternion.LookRotation(to.normalized)*Quaternion.Euler(0,180,0);}if(chaseClock<1f)status.text="¡COMIENZA LA CACERÍA!";}
        }
        else status.text="OCULTO EN LA VENTILACIÓN · SIN LÍMITE";
    }

    void ReadTouch()
    {
        moveInput=Vector2.zero;lookInput=Vector2.zero;
        for(int i=0;i<Input.touchCount;i++)
        {
            var touch=Input.GetTouch(i);Vector2 p=touch.position;
            if(touch.phase==TouchPhase.Began){if(p.x<Screen.width*.48f){draggingMove=true;moveStart=p;}else{draggingLook=true;lookStart=p;}}
            else if(touch.phase==TouchPhase.Ended||touch.phase==TouchPhase.Canceled){if(p.x<Screen.width*.5f)draggingMove=false;else draggingLook=false;}
            else if(draggingMove&&p.x<Screen.width*.55f)moveInput=Vector2.ClampMagnitude((p-moveStart)/85f,1);
            else if(draggingLook&&p.x>=Screen.width*.45f)lookInput+=touch.deltaPosition;
        }
        if(Input.touchCount==0){draggingMove=draggingLook=false;}
        yaw+=lookInput.x*.12f;pitch=Mathf.Clamp(pitch-lookInput.y*.10f,-78,78);cam.transform.localRotation=Quaternion.Euler(pitch,0,0);controller.transform.rotation=Quaternion.Euler(0,yaw,0);
        if(Input.GetKeyDown(KeyCode.E))ToggleHide();
        if(Input.GetKeyDown(KeyCode.Escape))ToggleHide();
        if(Input.GetKey(KeyCode.W))moveInput.y=1;if(Input.GetKey(KeyCode.S))moveInput.y=-1;if(Input.GetKey(KeyCode.A))moveInput.x=-1;if(Input.GetKey(KeyCode.D))moveInput.x=1;
    }
}
