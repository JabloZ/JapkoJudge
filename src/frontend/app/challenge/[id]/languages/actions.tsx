'use server';
import { getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { Manifest } from "@/lib/ClassTypes";
import { revalidatePath } from "next/cache";

type GetDataResult= {success:false, error: string} | {success:true, manifests:Manifest[]};
export async function GetLanguagesSupported(id:string):Promise<GetDataResult>{
    const session = await getSession();
        if (!session) {
            redirect("/");
        }
    const token=await getToken();
    if (!token){
        return {success:false, error:"no token"};
    }
    
    const response= await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}/returnLanguages`,{
        "method":"GET",
        "headers":{
            "Authorization":`Bearer ${token}`
        }
    });
    
    if (!response.ok){
        
        const err=await response.text();
        return{success:false,error:err};
    }
    const data=await response.json();
    const manifests = data.manifests ?? [];
    return{success:true,manifests:manifests as Manifest[]};
}

export async function DeleteLanguageHandle(id:string, language_id:string, path:string, prevState:any,formData:FormData){
    const session = await getSession();
        if (!session) {
            redirect("/");
        }
    const token=await getToken();
    if (!token){
        return {error:"no token"};
    }

    try{
       
        const response=await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}/deleteLanguageSupport/${language_id}`,{
            method:"POST",
            headers:{
                "Authorization":`Bearer ${token}`
            }
        });
        if (!response.ok){
            const err=await response.text();
             console.log(err);
            return {error:`Error! ${response.status}`}
        }
        
        const data=await response.json();
        
        
        //return {success: true, data};
    }
    catch(err){
        return {error: "Error: "+ (err as Error).message}
    }
    redirect(path);
}