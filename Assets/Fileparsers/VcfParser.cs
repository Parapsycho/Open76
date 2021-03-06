﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Fileparsers
{
    public class VcfParser
    {
        public class Vcf
        {
            public string VariantName { get; set; }
            public string VdfFilename { get; set; }
            public string VtfFilename { get; set; }
            public uint EngineType { get; set; }
            public uint SuspensionType { get; set; }
            public uint BrakesType { get; set; }
            public string WdfFrontFilename { get; set; }
            public string WdfMidFilename { get; set; }
            public string WdfBackFilename { get; set; }
            public uint ArmorFront { get; set; }
            public uint ArmorLeft { get; set; }
            public uint ArmorRight { get; set; }
            public uint ArmorRear { get; set; }
            public uint ChassisFront { get; set; }
            public uint ChassisLeft { get; set; }
            public uint ChassisRight { get; set; }
            public uint ChassisRear { get; set; }
            public uint ArmorOrChassisLeftToAdd { get; set; }
            public List<VcfWeapon> Weapons { get; set; }

            public Wdf FrontWheelDef { get; set; }
            public Wdf MidWheelDef { get; set; }
            public Wdf BackWheelDef { get; set; }
        }

        public class VcfWeapon
        {
            public MountPoint MountPoint { get; set; }
            public string GdfFilename { get; set; }
        }

        public enum MountPoint : uint
        {
            Dropper,
            FirstTop,
            Rear,
            SecondTop
        }

        public static Vcf ParseVcf(string filename)
        {
            var vcf = new Vcf();
            using (var br = new Bwd2Reader(filename))
            {
                br.FindNext("VCFC");

                vcf.VariantName = br.ReadCString(16);
                vcf.VdfFilename = br.ReadCString(13);
                vcf.VtfFilename = br.ReadCString(13);
                vcf.EngineType = br.ReadUInt32();
                vcf.SuspensionType = br.ReadUInt32();
                vcf.BrakesType = br.ReadUInt32();
                vcf.WdfFrontFilename = br.ReadCString(13);
                vcf.WdfMidFilename = br.ReadCString(13);
                vcf.WdfBackFilename = br.ReadCString(13);
                vcf.ArmorFront = br.ReadUInt32();
                vcf.ArmorLeft = br.ReadUInt32();
                vcf.ArmorRight = br.ReadUInt32();
                vcf.ArmorRear = br.ReadUInt32();
                vcf.ChassisFront = br.ReadUInt32();
                vcf.ChassisLeft = br.ReadUInt32();
                vcf.ChassisRight = br.ReadUInt32();
                vcf.ChassisRear = br.ReadUInt32();
                vcf.ArmorOrChassisLeftToAdd = br.ReadUInt32();

                br.FindNext("WEPN");
                vcf.Weapons = new List<VcfWeapon>();
                while (br.Current != null && br.Current.Name != "EXIT")
                {
                    var vcfWeapon = new VcfWeapon
                    {
                        MountPoint = (MountPoint)br.ReadUInt32(),
                        GdfFilename = br.ReadCString(13)
                    };
                    vcf.Weapons.Add(vcfWeapon);
                    br.Next();
                }

            }

            if (vcf.WdfFrontFilename.ToUpper() != "NULL")
                vcf.FrontWheelDef = WdfParser.ParseWdf(vcf.WdfFrontFilename);
            if (vcf.WdfMidFilename.ToUpper() != "NULL")
                vcf.MidWheelDef = WdfParser.ParseWdf(vcf.WdfMidFilename);
            if (vcf.WdfBackFilename.ToUpper() != "NULL")
                vcf.BackWheelDef = WdfParser.ParseWdf(vcf.WdfBackFilename);

            return vcf;
        }
    }
}
