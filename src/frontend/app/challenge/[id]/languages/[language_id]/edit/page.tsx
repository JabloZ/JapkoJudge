import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { EditLanguageForm } from "./EditLanguageForm";
export default async function EditLanguagePage({params}:{params:Promise<{id:string, language_id:string}>}) {
    //todo: check if viewer is author
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const{id, language_id}=await params;
    return <EditLanguageForm id={id} language_id={language_id}/>;
}