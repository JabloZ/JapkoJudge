'use server';

import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { Challenge, Manifest } from "@/lib/ClassTypes";
type ChallengesResult =
  | { success: true; challenge: Challenge}
  | { success: false; error: string };
  
export async function GetChallengeRequest({id}:{id:string}): Promise<ChallengesResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }

    const response=await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}`,{
        method:"GET",
        headers:{
            "Authorization":`Bearer ${token}`
        }
    })
    
    if (!response.ok){
        const resp=await response.text();
        return {success:false,error:resp};
    }

    const data=await response.json();
    
    
    const challenge = data ?? [];
    
    return{success:true,challenge: challenge as Challenge};

}
type LanguagesResult =
  | { success: true; manifests: Manifest[]}
  | { success: false; error: string };
  
export async function GetLanguagesSupported({id}:{id:string}): Promise<LanguagesResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }

    const response=await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}/returnLanguages`,{
        method:"GET",
        headers:{
            "Authorization":`Bearer ${token}`
        }
    })
    
    if (!response.ok){
        const resp=await response.text();
        return {success:false,error:resp};
    }

    const data=await response.json();
    
    
    const manifests = data?.manifests ?? [];
    
    return{success:true,manifests: manifests as Manifest[]};

}


export async function HandleAnswerPost(id:string,prevState:any,formData:FormData){
      const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }
    const code=formData.get("code");
    const languageName=formData.get("language");
    
    const response=await fetch(`${process.env.BACKEND_URL}/api/submissions/add/challenge/${id}`,{
        method:"POST",
        headers:{
            "Authorization":`Bearer ${token}`,
            "Content-Type":"application/json"
        },
        body:JSON.stringify({code,languageName})
    });
    
    if (!response.ok){
        
        const resp=await response.text();
        return {success:false,error:resp};
    }
    
    
    const data=await response.json();
    
    
    return{success:true};
}