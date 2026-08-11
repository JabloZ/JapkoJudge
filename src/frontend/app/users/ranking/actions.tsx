'use server';

import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { Language, Rank } from "@/lib/ClassTypes";
type RankingResult =
  | { success: true; ranking: Rank[] }
  | { success: false; error: string };
  
export async function GetRankingRequest(languageId: number): Promise<RankingResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }

    var top=100;
    
    const response=await fetch(`${process.env.BACKEND_URL}/api/ranking/top=${top},language=${languageId}`,{
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
    const ranking = data.ranking ?? [];
    
    return{success:true,ranking: ranking as Rank[]};

}


type LanguagesResult =
  | { success: true; languages: Language[] }
  | { success: false; error: string };

export async function GetLanguages(): Promise<LanguagesResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }

    const response=await fetch(`${process.env.BACKEND_URL}/api/languages`,{
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
    const languages = data.languages ?? [];
    console.log(languages);
    return{success:true,languages: languages as Language[]};

}