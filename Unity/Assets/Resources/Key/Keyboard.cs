using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum Kana
{
    A, I, U, E, O,
    Ka, Ki, Ku, Ke, Ko,
    Sa, Si, Su, Se, So,
    Ta, Ti, Tu, Te, To,
    Na, Ni, Nu, Ne, No,
    Ha, Hi, Hu, He, Ho,
    Ma, Mi, Mu, Me, Mo,
    Ya, LKakko, Yu, RKakko, Yo,
    Ra, Ri, Ru, Re, Ro,
    Wa, Wo, N, Bar, Nami,
    Ten, Maru, Hatena, Bikkuri, Santen,
    None,
    Dakuten,
}


public class Keyboard : MonoBehaviour
{
    public static string[] kanaStrs = new string[]{
        "あ", "い", "う", "え", "お", 
        "か", "き", "く", "け", "こ", 
        "さ", "し", "す", "せ", "そ", 
        "た", "ち", "つ", "て", "と", 
        "な", "に", "ぬ", "ね", "の", 
        "は", "ひ", "ふ", "へ", "ほ", 
        "ま", "み", "む", "め", "も", 
        "や", "（", "ゆ", "）", "よ", 
        "ら", "り", "る", "れ", "ろ", 
        "わ", "を", "ん", "ー", "～", 
        "、", "。", "？", "！", "…"
    };

    public Text inputText;
    private StringBuilder sb = new StringBuilder("", 50);
    public GameObject select1;
    public GameObject select2;

    private static Texture2D activeTexture;
    private static Texture2D inactiveTexture;
    
    private KeyGroup[] keyGroups = new KeyGroup[(int)Kana.None/5];
    private Key dakuten;

    // Start is called before the first frame update
    void Start()
    {
        activeTexture = Resources.Load<Texture2D>("Key/Textures/active");
        inactiveTexture = Resources.Load<Texture2D>("Key/Textures/inactive");
        select1 = GameObject.Find("Select1");
        select2 = GameObject.Find("Select2");

        for (int mainKana = 0; mainKana < (int)Kana.None; mainKana+=5)
        {
            Debug.Log(((Kana)mainKana).ToString());
            keyGroups[mainKana/5] = new KeyGroup((Kana)mainKana);
        }
        dakuten = new Key(Kana.Dakuten, GameObject.Find("Dakuten"));

        inputText = GameObject.Find("InputText").GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int mainKana = getInputOfLeftController();
        int rightInput = getInputOfRightController();

        if(rightInput == 10){
            onBackspace();
        }else{ 
            // if a key group is selected
            if(mainKana < (int)Kana.None){
                if(rightInput >= 5){
                    onInput((Kana)(mainKana + rightInput % 5));
                    rightInput = -1;
                }
            }else if(rightInput == 5){
                onDakuten();
            }
            selectKey(mainKana, rightInput);
        }
    }

    int getInputOfLeftController()
    {
        Vector2 stick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        bool trigger = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
        if(stick.x == 0f && stick.y == 0f){
            if(trigger){
                return (int)Kana.None + 1;
            }else{
                return (int)Kana.None;
            }
        }else if(trigger){
            if(stick.normalized.x > 0.5f){
                if (stick.normalized.y > 0){
                    return (int)Kana.Ra;
                }else{
                    return (int)Kana.Ten;
                }
            }else if(stick.normalized.x < -0.5f){
                if (stick.normalized.y > 0){
                    return (int)Kana.Ma;
                }else{
                    return (int)Kana.Wa;
                }
            }else{
                if (stick.normalized.y > 0){
                    return (int)Kana.Ya;
                }else{
                    return (int)Kana.Wa;
                }
            }
        }else{
            if(stick.normalized.x > 0.5f){
                if (stick.normalized.y > 0){
                    return (int)Kana.Sa;
                }else{
                    return (int)Kana.Ha;
                }
            }else if(stick.normalized.x < -0.5f){
                if (stick.normalized.y > 0){
                    return (int)Kana.A;
                }else{
                    return (int)Kana.Ta;
                }
            }else{
                if (stick.normalized.y > 0){
                    return (int)Kana.Ka;
                }else{
                    return (int)Kana.Na;
                }
            }
        }
    }

    int getInputOfRightController()
    {
        bool up = OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp);
        bool down = OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown);
        bool left = OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft);
        bool right = OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight);
        bool trigger = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);

        bool upUp = OVRInput.GetUp(OVRInput.Button.SecondaryThumbstickUp);
        bool downUp = OVRInput.GetUp(OVRInput.Button.SecondaryThumbstickDown);
        bool leftUp = OVRInput.GetUp(OVRInput.Button.SecondaryThumbstickLeft);
        bool rightUp = OVRInput.GetUp(OVRInput.Button.SecondaryThumbstickRight);
        bool triggerUp = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);

        bool bDown = OVRInput.GetDown(OVRInput.Button.Two);
        
        if(trigger){
            return 0;
        }else if(left){
            return 1;
        }else if(up){
            return 2;
        }else if(right){
            return 3;
        }else if(down){
            return 4;
        }else if(triggerUp){
            return 5;
        }else if(leftUp){
            return 6;
        }else if(upUp){
            return 7;
        }else if(rightUp){
            return 8;
        }else if(downUp){
            return 9;
        }else if(bDown){
            return 10;
        }else{
            return -1;
        }
    }

    void selectKey(int mainKana, int rightInput){
        int activeGroupIdx = mainKana/5; 
        if(activeGroupIdx == keyGroups.Length){
            // if no key group is selected
            foreach (var item in keyGroups){
                item.SetActive(true);
                item.SetChildsActive(false);
            }
            select1.SetActive(mainKana == (int)Kana.None);
            select2.SetActive(mainKana == (int)Kana.None + 1);
            
            dakuten.SetActive(true);
            dakuten.SetHighlighted(rightInput==0);
        }else{
            var activeGroup = keyGroups[activeGroupIdx];
            foreach (var item in keyGroups){
                if(item == activeGroup){
                    item.SetActive(true);
                    item.SetChildsActive(true);
                }else{
                    item.SetActive(false);
                }
            }
            dakuten.SetActive(false);
            keyGroups[activeGroupIdx].HighlightChild(rightInput);
        }
    }

    void onInput(Kana kana){
        sb.Append(kanaStrs[(int)kana]);
        inputText.text = sb.ToString();
    }
    void onBackspace(){
        sb.Length--;
        inputText.text = sb.ToString();
    }
    void onDakuten(){
        char result = changeKanaType(sb[sb.Length-1]);
        if(result!=sb[sb.Length-1]){
            sb[sb.Length-1] = result;
            inputText.text =sb.ToString();
        }
    }

    char changeKanaType(char c){
        if(('か' <= c && c <= 'ぢ') || ('ゃ' <= c && c <= 'よ')){
            if(c % 2 ==0){
                c--;
            }else{
                c++;
            }
        }else if('て' <= c && c <= 'ど' ){
            if(c % 2 ==0){
                c++;
            }else{
                c--;
            }
        }else if(('は' <= c && c <= 'ぽ') || ('っ' <= c && c <= 'づ')){
            if(c % 3 <= 1){
                c++;
            }else{
                c -= (char)2;
            }
        }
        return c;
    }
    
    public class Key
    {
        public Kana kana;
        private GameObject gameObject;
        private Vector3 defaultPosition;
        private static float popDepth = 0.1f;
        private Material[] materials;
        private int[] textureIDs = new int[2];
        private bool highlighted = false;
        public Key(Kana kana, GameObject gameObject){
            this.kana = kana;
            this.gameObject = gameObject;
            this.defaultPosition = gameObject.transform.position;
            this.materials = this.gameObject.GetComponent<Renderer>().materials;
            this.textureIDs[0] = this.materials[0].GetTexturePropertyNameIDs()[0];
            this.textureIDs[1] = this.materials[1].GetTexturePropertyNameIDs()[0];
            materials[0].SetTexture(textureIDs[0], inactiveTexture);
            materials[1].SetTexture(textureIDs[1], Resources.Load<Texture2D>("Key/Textures/" + kana.ToString()));
        }

        public void SetHighlighted(bool flag){
            if(highlighted == flag)return;
            highlighted = flag;
            if(flag){
                gameObject.transform.position = defaultPosition + Vector3.back * popDepth;
                materials[0].SetTexture(textureIDs[0], activeTexture); 
            }else{
                gameObject.transform.position = defaultPosition;
                materials[0].SetTexture(textureIDs[0], inactiveTexture); 
            }
        }

        public void SetActive(bool flag){
            gameObject.SetActive(flag);
        }
    }

    public class KeyGroup
    {
        public Key[] keys = new Key[5];
        
        public KeyGroup(Kana mainKana){
            keys[0] = new Key(mainKana, GameObject.Find(mainKana.ToString()));
            for(int i = 1; i < 5; i++){
                var kana = (Kana)((int)mainKana + i);
                keys[i] = new Key(kana, GameObject.Find(kana.ToString()));
                keys[i].SetActive(false);
            }
        }

        public void SetChildsActive(bool flag){
            for(int i = 1; i < 5; i++){
                keys[i].SetActive(flag);
            }
        }

        public void SetActive(bool flag){
            keys[0].SetActive(flag);
        }
        
        public void HighlightChild(int idx){
            for(int i = 0; i < 5; i++){
                keys[i].SetHighlighted(i == idx);
            }
        }
    }
}

