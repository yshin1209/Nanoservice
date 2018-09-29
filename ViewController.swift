//
//  ViewController.swift
//  Hyponatremia II
//
//  Created by Jay Shin on 8/9/18.
//  Copyright © 2018 Eun-Jai Shin. All rights reserved.
//

import UIKit
import Alamofire
import SwiftyJSON


class ViewController: UIViewController {

    /*The CSMLAB7 get/set value URLS are posted here for reference:
     
    let setValueURL : String = "https://csmlab7.uconn.edu/setvalue"
    let getValueURL : String = "https://csmlab7.uconn.edu/getvalue" */
    
    
    //These are the user-interactive compartments of the main app storyboard
    
    @IBOutlet weak var patientNameInputText: UITextField!
    @IBOutlet weak var patientSodiumLevelText: UITextField!
    
    @IBOutlet weak var clickToSendButton: UIButton!
    
    
    //This is the text box that will show true/false value of the hyponatremia check
    @IBOutlet weak var sodiumDiagnosisResult: UILabel!
    
    
    //Setting up a new patient model
    let thisPatientModel = PatientModel()
    
    
    //INITIAL PREPARATIONS
    /*********************************************************/

    //Initial data input from usre via UITextFields
    func getPatientDataInput() {
        
        thisPatientModel.actorID = String(patientNameInputText.text!)
        thisPatientModel.value = Float(patientSodiumLevelText.text!)!
        
        print("Got patient information: \(thisPatientModel.actorID) and \(thisPatientModel.value)")
        
        
        //Re-organizing patient data in dictionary (JSON) format
        
        let organizedJSON : [String : Any] = [
            "actorId": thisPatientModel.actorID,
            "variable" : thisPatientModel.variable,
            "value" : thisPatientModel.value,
            "publish" : thisPatientModel.publish
            
        ]
        
        thisPatientModel.finalpatientJSON = organizedJSON

    }
    
    
    
    
    //SET + GET
    /*********************************************************/
    
    
    
    //This function will send patient JSON to setValue link and add information to the collective database
    func sendToCSMLAB7(parameters: [String : Any]) {
        print("sending patientJSON information to CSMLAB7/setvalue")
        
        //Alamofire function request: sending .post to url
        Alamofire.request("http://csmlab7.uconn.edu/setValue", method: .post, parameters: parameters, encoding: JSONEncoding.default)
            .responseData {
                
                response in
                
                if response.result.isSuccess {
                    print("Successfully sent data to CSMLAB7.")
                    
                }
                    
                // in case of error
                else {
                    print("There is an error- \(response.result.error)")
                    self.sodiumDiagnosisResult.text = "Connection Error"
                }
        }
        
    }
    
    
    
    
    //This function will send patient JSON (ID and desired variable: hyponatremia) in order to receive diagnosis
    func getFromCSMLAB7(parameters: [String:Any]) {
        print("retrieving patientJSON information to CSMLAB7/getvalue")
        
        //Alamofire function request: sending .post to url
        Alamofire.request("http://csmlab7.uconn.edu/getValue", method: .post, parameters: parameters, encoding: JSONEncoding.default)
            .responseData {
                
                response in
                
                if response.result.isSuccess {
                    print("Successfully retrieved data from CSMLAB7.")
                    
                    let finalJSON : JSON = JSON(response.result.value!)
                    
                    self.updateSodiumResult(json: finalJSON)
                }
                    
                // in case of error
                else {
                    print("There is an error- \(response.result.error)")
                    self.sodiumDiagnosisResult.text = "Connection Error"
                }
        }
        
    }
    
    //This is the function for updating the sodium result data once set/get has been finished
    //Hyponatremia diagnosis in JSON format is converted to String
    func updateSodiumResult(json:JSON) {
        
        let hyponatremiaResult = json["value"]
        
        print("\(hyponatremiaResult)")
        
        if hyponatremiaResult != JSON.null {
            
            thisPatientModel.myHyponatremiaResult = hyponatremiaResult.stringValue
            updateUIWithResults()
        }
        
        else {
            print("Data could not be retrieved.")
        }
        
    }
    
    
    //UI Updates
    /*********************************************************/
    
    //Displaying results on UI
    func updateUIWithResults() {
        
        sodiumDiagnosisResult.text = thisPatientModel.myHyponatremiaResult
        
    }
    
    
    
    override func viewDidLoad() {
        super.viewDidLoad()
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }


    /* Below is the trigger function to this app: information is not sent to the csmlab7 url until "Click to Send" button is pressed */
    @IBAction func sendButtonPressed(_ sender: UIButton) {
        
        if sender.tag == 0 {
            getPatientDataInput()
            sendToCSMLAB7(parameters: thisPatientModel.finalpatientJSON)
            let hyponatremiaCheckJSON : [String:Any] = ["actorId" : self.thisPatientModel.actorID,
                                                     "variable" : "hyponatremia"]
            
            DispatchQueue.main.asyncAfter(deadline: .now() + 5) {
                self.getFromCSMLAB7(parameters: hyponatremiaCheckJSON)
            }
        }
    }
    

    


}

