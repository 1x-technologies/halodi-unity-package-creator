using UnityEngine;

namespace Halodi.PackageCreator
{
    [System.Serializable]
    public class SPDXLicense
    {
        public string licenseId;
    }

    [System.Serializable]
    public class SPDXLicenseList
    {
        
        
        public SPDXLicense[] licenses;


        public static SPDXLicenseList Load()
        {
            var spdxJson = Resources.Load<TextAsset>("halodi-unity-package-creator/spdx-license-list");

            if(spdxJson == null)
            {
                SPDXLicenseList empty = new SPDXLicenseList();
                SPDXLicense proprietary = new SPDXLicense();
                proprietary.licenseId = "proprietary";
                empty.licenses = new SPDXLicense[] { proprietary };

                return empty;
            }
            else
            {
                return JsonUtility.FromJson<SPDXLicenseList>(spdxJson.text);
            }
        }



        public string[] ToStringArray()
        {
            string[] stringArray = new string[licenses.Length];
            for(int i = 0; i < stringArray.Length; i++)
            {
                stringArray[i] = licenses[i].licenseId;
            }

            return stringArray;
        }

    }

}