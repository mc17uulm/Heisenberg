using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config{

    // Konfiguration für Durchgang

	public static IDictionary<string, object> Load()
    {
        IDictionary<string, object> config = new Dictionary<string, object>();

        // Positionen der Ziele
        config["start_position"] = new Vector3(0,0,0);
        config["positions"] = new Vector3[]
        {
            new Vector3(-280,180,0),
            new Vector3(-93.5f,180,0),
            /**new Vector3(93.5f,180,0),
            new Vector3(280,180,0),

            new Vector3(-280,0,0),
            new Vector3(-93.5f,0,0),
            new Vector3(93.5f,0,0),
            new Vector3(280,0,0),

            new Vector3(-280,-180,0),
            new Vector3(-93.5f,-180,0),
            new Vector3(93.5f,-180,0),
            new Vector3(280,-180,0)*/
        };

        // Wie oft soll eine Position wiederholt werden?
        config["repeat"] = 2;
        // Durchgänge
        config["tries"] = 1;
        // Sollen die Positionen random oder nach Reihenfolge erscheinen
        config["random"] = true;
        // Größe des Zieles (width = heigth) in px
        config["dimension"] = 15;
        // Entfernung von Kamera zum Ziel (z-Koordinate)
        config["distance"] = 8;
        // Letzte Position oder Durchschnitt anzeigen lassen (Standart-Durchschnitt: min. 50 letzte Positionen)
        config["last_position"] = false;
        // SaveFile
        config["savefile"] = @"C:\Users\mi-vr\Desktop\Heisenberg\savefile.csv";


        return config;
    }
}
