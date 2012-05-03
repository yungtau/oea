﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OEA.Web;
using OEA.Web.Json;
using OEA.MetaModel;
using System.Reflection;

namespace Web.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ModuleRootJson = IndexModuleJsonConverter.GetJson();

            return View();
        }

        public ActionResult EntityModule()
        {
            ViewBag.Type = Request.QueryString["type"];
            ViewBag.ViewName = Request.QueryString["viewName"];
            ViewBag.IsAggt = Request.GetQueryStringOrDefault("isAggt", 1) == 1;
            return View();
        }

        public ActionResult Module()
        {
            ViewBag.Module = Request.QueryString["module"];
            return View();
        }

        public ActionResult ExtTest()
        {
            return View();
        }

        public ActionResult ExtTestJS()
        {
            return JavaScript(@"
{
    'totalCount': 8,
    'entities':[
    {
        task:'Project: Shopping',
        duration:13.25,
        user:'Tommy Maintz',
        iconCls:'task-folder',
        entities:[{
            task:'Housewares',
            duration:1.25,
            user:'Tommy Maintz',
            iconCls:'task-folder',
            entities:[{
                task:'Kitchen supplies',
                duration:0.25,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task:'Groceries',
                duration:.4,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task:'Cleaning supplies',
                duration:.4,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task: 'Office supplies',
                duration: .2,
                user: 'Tommy Maintz',
                leaf: true,
                iconCls: 'task'
            }]
        }, {
            task:'Remodeling',
            duration:12,
            user:'Tommy Maintz',
            iconCls:'task-folder',
            expanded: true,
            entities:[{
                task:'Retile kitchen',
                duration:6.5,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task:'Paint bedroom',
                duration: 2.75,
                user:'Tommy Maintz',
                iconCls:'task-folder',
                entities: [{
                    task: 'Ceiling',
                    duration: 1.25,
                    user: 'Tommy Maintz',
                    iconCls: 'task',
                    leaf: true
                }, {
                    task: 'Walls',
                    duration: 1.5,
                    user: 'Tommy Maintz',
                    iconCls: 'task',
                    leaf: true
                }]
            },{
                task:'Decorate living room',
                duration:2.75,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task: 'Fix lights',
                duration: .75,
                user: 'Tommy Maintz',
                leaf: true,
                iconCls: 'task'
            }, {
                task: 'Reattach screen door',
                duration: 2,
                user: 'Tommy Maintz',
                leaf: true,
                iconCls: 'task'
            }]
        }]
    },{
        task:'Project: Testing',
        duration:2,
        user:'Core Team',
        iconCls:'task-folder',
        entities:[{
            task: 'Mac OSX',
            duration: 0.75,
            user: 'Tommy Maintz',
            iconCls: 'task-folder',
            entities: [{
                task: 'FireFox',
                duration: 0.25,
                user: 'Tommy Maintz',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Safari',
                duration: 0.25,
                user: 'Tommy Maintz',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Chrome',
                duration: 0.25,
                user: 'Tommy Maintz',
                iconCls: 'task',
                leaf: true
            }]
        },{
            task: 'Windows',
            duration: 3.75,
            user: 'Darrell Meyer',
            iconCls: 'task-folder',
            entities: [{
                task: 'FireFox',
                duration: 0.25,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Safari',
                duration: 0.25,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Chrome',
                duration: 0.25,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            },{
                task: 'Internet Exploder',
                duration: 3,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            }]
        },{
            task: 'Linux',
            duration: 0.5,
            user: 'Aaron Conran',
            iconCls: 'task-folder',
            entities: [{
                task: 'FireFox',
                duration: 0.25,
                user: 'Aaron Conran',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Chrome',
                duration: 0.25,
                user: 'Aaron Conran',
                iconCls: 'task',
                leaf: true
            }]
        }]
    }


]}
");
        }

        public ActionResult ExtTestJS2()
        {
            return JavaScript(@"
{'text':'.', 'children': [
    {
        task:'Project: Shopping',
        duration:13.25,
        user:'Tommy Maintz',
        iconCls:'task-folder',
        expanded: true,
        children:[{
            task:'Housewares',
            duration:1.25,
            user:'Tommy Maintz',
            iconCls:'task-folder',
            children:[{
                task:'Kitchen supplies',
                duration:0.25,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task:'Groceries',
                duration:.4,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task:'Cleaning supplies',
                duration:.4,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task: 'Office supplies',
                duration: .2,
                user: 'Tommy Maintz',
                leaf: true,
                iconCls: 'task'
            }]
        }, {
            task:'Remodeling',
            duration:12,
            user:'Tommy Maintz',
            iconCls:'task-folder',
            expanded: true,
            children:[{
                task:'Retile kitchen',
                duration:6.5,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task:'Paint bedroom',
                duration: 2.75,
                user:'Tommy Maintz',
                iconCls:'task-folder',
                children: [{
                    task: 'Ceiling',
                    duration: 1.25,
                    user: 'Tommy Maintz',
                    iconCls: 'task',
                    leaf: true
                }, {
                    task: 'Walls',
                    duration: 1.5,
                    user: 'Tommy Maintz',
                    iconCls: 'task',
                    leaf: true
                }]
            },{
                task:'Decorate living room',
                duration:2.75,
                user:'Tommy Maintz',
                leaf:true,
                iconCls:'task'
            },{
                task: 'Fix lights',
                duration: .75,
                user: 'Tommy Maintz',
                leaf: true,
                iconCls: 'task'
            }, {
                task: 'Reattach screen door',
                duration: 2,
                user: 'Tommy Maintz',
                leaf: true,
                iconCls: 'task'
            }]
        }]
    },{
        task:'Project: Testing',
        duration:2,
        user:'Core Team',
        iconCls:'task-folder',
        children:[{
            task: 'Mac OSX',
            duration: 0.75,
            user: 'Tommy Maintz',
            iconCls: 'task-folder',
            children: [{
                task: 'FireFox',
                duration: 0.25,
                user: 'Tommy Maintz',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Safari',
                duration: 0.25,
                user: 'Tommy Maintz',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Chrome',
                duration: 0.25,
                user: 'Tommy Maintz',
                iconCls: 'task',
                leaf: true
            }]
        },{
            task: 'Windows',
            duration: 3.75,
            user: 'Darrell Meyer',
            iconCls: 'task-folder',
            children: [{
                task: 'FireFox',
                duration: 0.25,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Safari',
                duration: 0.25,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Chrome',
                duration: 0.25,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            },{
                task: 'Internet Exploder',
                duration: 3,
                user: 'Darrell Meyer',
                iconCls: 'task',
                leaf: true
            }]
        },{
            task: 'Linux',
            duration: 0.5,
            user: 'Aaron Conran',
            iconCls: 'task-folder',
            children: [{
                task: 'FireFox',
                duration: 0.25,
                user: 'Aaron Conran',
                iconCls: 'task',
                leaf: true
            }, {
                task: 'Chrome',
                duration: 0.25,
                user: 'Aaron Conran',
                iconCls: 'task',
                leaf: true
            }]
        }]
    }
]}
");
        }
    }

    internal class IndexModuleJson : JsonModel
    {
        public string text, model, url, viewName;

        public bool leaf;

        public List<IndexModuleJson> children = new List<IndexModuleJson>();

        protected override void ToJson(LiteJsonWriter json)
        {
            json.WritePropertyIf("url", url);
            json.WritePropertyIf("model", model);
            json.WritePropertyIf("viewName", viewName);
            if (leaf) { json.WritePropertyIf("leaf", true); }
            if (children.Count > 0) { json.WritePropertyIf("children", children); }
            json.WriteProperty("text", text, true);
        }
    }

    internal static class IndexModuleJsonConverter
    {
        public static string GetJson()
        {
            var result = new IndexModuleJson();

            var roots = CommonModel.Modules.GetRoots();
            foreach (var root in roots)
            {
                var rootJson = ToJson(root);
                result.children.Add(rootJson);
            }

            return result.ToJsonString();
        }

        private static IndexModuleJson ToJson(ModuleMeta module)
        {
            var mJson = new IndexModuleJson
            {
                text = module.Label,
                leaf = module.Children.Count == 0
            };

            if (module.HasUI)
            {
                if (module.IsCustomUI)
                {
                    mJson.url = module.CustomUI;
                }
                else
                {
                    mJson.model = ClientEntities.GetClientName(module.EntityType);
                    mJson.viewName = module.AggtBlocksName;
                }
            }

            foreach (var child in module.Children)
            {
                var childJson = ToJson(child);
                mJson.children.Add(childJson);
            }

            return mJson;
        }
    }
}