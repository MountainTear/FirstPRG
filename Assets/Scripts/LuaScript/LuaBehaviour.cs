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
    //这是一个序列化标签，说明这个类被序列化，之后可被Unity存储并重构，会出现在Unity的Inspector面板上
    [System.Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    //这个标签说明在引用的Lua代码里会调用C#的函数
    [LuaCallCSharp]
    //这是一个继承自Unity脚本基类的类，使其可作为Unity脚本组件挂靠在游戏对象上
    public class LuaBehaviour : MonoBehaviour
    {
        //因为存储Lua代码的文件为txt格式，所以这里用文本资源引入
        public TextAsset luaScript;
        //可以通过外部设置将Unity组件引入到Lua中，只需先修改size，然后修改name和value即可
        public Injection[] injections;

        //总体的一个Lua环境，各个脚本环境的基础
        internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
        internal static float lastGCTime = 0;
        //一个时间变量，用来GC的
        internal const float GCInterval = 1;//1 second 

        //三个Action（无返回值和参数的delegate），正好用来存对应名字的Lua函数
        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;

        //脚本环境，也就是每一个挂靠在游戏对象上的脚本组件，即脚本对象，都有一个独立的脚本环境，但总体都是由类的一个luaEnv new出来的
        private LuaTable scriptEnv;

        void Awake()
        {
            scriptEnv = luaEnv.NewTable();

            //为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            //脚本环境的元表的__index方法为总体环境luaEnv.Global，也就是Lua的_G，保存了Lua的全局变量及全局函数
            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            //将lua脚本的self设为当前脚本指针，就可以通过该指针获取游戏对象的component
            scriptEnv.Set("self", this);
            
            //将外部设置的所有Unity组件传入到Lua对应名字的对象中
            foreach (var injection in injections)
            {
                scriptEnv.Set(injection.name, injection.value);
            }
            //执行将TextAsset对象luaScript的Lua代码，代码块名为LuaTestScript，在scriptEnv中执行
            luaEnv.DoString(luaScript.text, "LuaTestScript", scriptEnv);

            //Lua里有awake就执行一下
            Action luaAwake = scriptEnv.Get<Action>("awake");
            //获取Lua里的仨函数并传给本地Action
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
            //定时调用，用来清除Lua的未手动释放的LuaBase对象（比如：LuaTable， LuaFunction），以及其它一些事情
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
            //释放脚本环境
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            scriptEnv.Dispose();
            injections = null;
        }
    }
   
}