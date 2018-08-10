% Hyponaremia Differential Diagnosi MATLAB Client
% Yong-Jun Shin (2018)

clc
clear all

patientId = 'pt17432'; % New patient ID (Service Fabric Actor ID) 

% Initialization
% This events are not published ('no').
setValue (patientId, 'hypotonicity', 'not applicable', 'no');  
setValue (patientId, 'hypertonicity', 'not applicable', 'no'); 
setValue (patientId, 'hypovolemia', 'not applicable', 'no'); 
setValue (patientId, 'highUrineSodium', 'not applicable', 'no'); 
setValue (patientId, 'hypervolemia', 'not applicable', 'no'); 
setValue (patientId, 'hyponatremiaDDx', 'not applicable', 'no'); 
setValue (patientId, 'serumOsmolarity', 383, 'no');  
setValue (patientId, 'volumeStatus', 'euvolemic', 'no');  
setValue (patientId, 'urineSodium', 43, 'no'); 

% Set the value of "bloodSodium. This event is published ('yes')
setValue (patientId, 'bloodSodium',97, 'yes');  

% Pause 5 seconds (sufficient time) for all the operations to complete
pause(5)

% Dispaly the result.
hyponatremia = getValue (patientId, 'hyponatremia');    
hyponatremiaValue = extractfield(hyponatremia,"value");
disp (['hyponatremia' hyponatremiaValue]);

hypotonicity = getValue (patientId, 'hypotonicity'); 
hypotonicityValue = extractfield(hypotonicity,"value"); 
disp (['hypotonicity' hypotonicityValue]);

hypertonicity = getValue (patientId, 'hypertonicity'); 
hypertonicityValue = extractfield(hypertonicity,"value"); 
disp (['hypertonicity' hypertonicityValue]);
hypovolemia = getValue (patientId, 'hypovolemia'); 
hypovolemiaValue = extractfield(hypovolemia,"value"); 
disp (['hypovolemia' hypovolemiaValue]);

highUrineSodium = getValue (patientId, 'highUrineSodium'); 
highUrineSodiumValue = extractfield(highUrineSodium,"value"); 
disp (['highUrineSodium' highUrineSodiumValue]);

hypervolemia = getValue (patientId, 'hypervolemia');
hypervolemiaValue = extractfield(hypervolemia,"value"); 
disp (['hypervolemia' hypervolemiaValue]);

hyponatremiaDDx = getValue (patientId, 'hyponatremiaDDx'); 
hyponatremiaDDxValue = extractfield(hyponatremiaDDx,"value"); 
disp (['hyponatremiaDDx' hyponatremiaDDxValue]);

%getValue
function value = getValue(actorId, variable)
%baseNanoserviceUri = 'http://nanoservice3.eastus.cloudapp.azure.com';
baseNanoserviceUri = 'http://csmlab7.uconn.edu';
requestUri = [baseNanoserviceUri '/getValue'];
data = struct('actorId', actorId, 'variable', variable);
options = weboptions('MediaType','application/json', 'Timeout', 1000);
value = webwrite(requestUri, data, options);
end

%setValue
function response = setValue(actorId, variable, value, publish)
%baseNanoserviceUri = 'http://nanoservice3.eastus.cloudapp.azure.com';
baseNanoserviceUri = 'http://csmlab7.uconn.edu';
requestUri = [baseNanoserviceUri '/setValue'];
data = struct('actorId', actorId, 'variable', variable, 'value', value, 'publish', publish);
options = weboptions('MediaType', 'application/json', 'Timeout', 1000);
response = webwrite(requestUri, data, options);
end