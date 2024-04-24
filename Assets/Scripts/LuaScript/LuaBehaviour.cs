/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using UnityEngine.SceneManagement;

namespace XLuaTest
{
    //����һ�����л���ǩ��˵������౻���л���֮��ɱ�Unity�洢���ع����������Unity��Inspector�����
    [System.Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    //�����ǩ˵�������õ�Lua����������C#�ĺ���
    [LuaCallCSharp]
    //����һ���̳���Unity�ű�������࣬ʹ�����ΪUnity�ű�����ҿ�����Ϸ������
    public class LuaBehaviour : MonoBehaviour
    {
        //��Ϊ�洢Lua������ļ�Ϊtxt��ʽ�������������ı���Դ����
        public TextAsset luaScript;
        //����ͨ���ⲿ���ý�Unity������뵽Lua�У�ֻ�����޸�size��Ȼ���޸�name��value����
        public Injection[] injections;

        //�����һ��Lua�����������ű������Ļ���
        internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
        internal static float lastGCTime = 0;
        //һ��ʱ�����������GC��
        internal const float GCInterval = 1;//1 second 

        //����Action���޷���ֵ�Ͳ�����delegate���������������Ӧ���ֵ�Lua����
        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;

        //�ű�������Ҳ����ÿһ���ҿ�����Ϸ�����ϵĽű���������ű����󣬶���һ�������Ľű������������嶼�������һ��luaEnv new������
        private LuaTable scriptEnv;

        void Awake()
        {
            scriptEnv = luaEnv.NewTable();

            //Ϊÿ���ű�����һ�������Ļ�������һ���̶��Ϸ�ֹ�ű���ȫ�ֱ�����������ͻ
            //�ű�������Ԫ���__index����Ϊ���廷��luaEnv.Global��Ҳ����Lua��_G��������Lua��ȫ�ֱ�����ȫ�ֺ���
            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            //��lua�ű���self��Ϊ��ǰ�ű�ָ�룬�Ϳ���ͨ����ָ���ȡ��Ϸ�����component
            scriptEnv.Set("self", this);
            
            //���ⲿ���õ�����Unity������뵽Lua��Ӧ���ֵĶ�����
            foreach (var injection in injections)
            {
                scriptEnv.Set(injection.name, injection.value);
            }
            //ִ�н�TextAsset����luaScript��Lua���룬�������ΪLuaTestScript����scriptEnv��ִ��
            luaEnv.DoString(luaScript.text, "LuaTestScript", scriptEnv);

            //Lua����awake��ִ��һ��
            Action luaAwake = scriptEnv.Get<Action>("awake");
            //��ȡLua�����������������Action
            scriptEnv.Get("start", out luaStart);
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("ondestroy", out luaOnDestroy);

            if (luaAwake != null)
            {
                luaAwake();
            }
        }

        // Use this for initialization
        void Start()
        {
            if (luaStart != null)
            {
                luaStart();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (luaUpdate != null)
            {
                luaUpdate();
            }
            //��ʱ���ã��������Lua��δ�ֶ��ͷŵ�LuaBase���󣨱��磺LuaTable�� LuaFunction�����Լ�����һЩ����
            if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                LuaBehaviour.lastGCTime = Time.time;
            }
        }

        void OnDestroy()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }
            //�ͷŽű�����
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            scriptEnv.Dispose();
            injections = null;
        }
    }
   
}