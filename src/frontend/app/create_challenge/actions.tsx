'use server';
import { getToken } from "@/lib/session";
export async function SendChallengeRequest(){
    const token=getToken();
    if (token!=null){

    }
    //todo 
    const response=await fetch(`${process.env.BACKEND_URL}/api/createChallengeRequest`,{
        method:"POST",
        headers:{
            "Authorization":`Bearer ${token}`,
            "Content-Type":"application/json"
        },
        
    );
    
}